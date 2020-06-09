/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Buffer;
using Pioneer.Framework;
using Pioneer.Network;
using Pioneer.Sync;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Pioneer
{
    sealed partial class World : ISerializer
    {
        private Creator defaultCreator = null;
        private readonly Dictionary<ulong, IActor> players = new Dictionary<ulong, IActor>();
        private readonly Queue<SyncAction> syncActions = new Queue<SyncAction>();
        private readonly Queue<Action> deferActions = new Queue<Action>();
        private readonly Dictionary<ulong, IPeer> peers = new Dictionary<ulong, IPeer>();
        private ISocket socket = null;
        private object syncObject = new object();

        public event Action OnLoading;
        public event Action<Exception> OnClosed;

        public WorldMode Mode { get; }

        public bool Marshal(object packet, IBufWriter writer)
        {
            var p = (Packet)packet;
            return p.Marshal(writer);
        }

        public bool Unmarshal(IBufReader reader, out object obj)
        {
            return ((Packet)(obj = new Packet(0))).Unmarshal(reader);
        }

        public void Start(string nsp = null)
        {
            if (WorldMode.Standalone == this.Mode)
            {
                EnqueueDeferAction(() => { this.OnLoading?.Invoke(); });
            }
            else
            {
                if (null == nsp)
                {
                    throw new ArgumentNullException(string.Format("nsp should not be NULL under {0} mode!", this.Mode));
                }

                Reset();

                this.socket = SocketFactory.Create(nsp, this);
                this.socket.OnConnected += OnPeerConnected;
                this.socket.OnClosed += OnPeerClosed;
                this.socket.OnMessage += OnPeerMessage;

                switch (this.Mode)
                {
                case WorldMode.Client:
                    this.socket.Dial();
                    break;
                case WorldMode.Server:
                    this.socket.Listen();
                    EnqueueDeferAction(() => { this.OnLoading?.Invoke(); });
                    break;
                }
            }
        }

        public void Stop()
        {
            EnqueueDeferAction(() => { this.OnClosed?.Invoke(null); });
        }

        public void NotifyLoaded()
        {
            switch (this.Mode)
            {
            case WorldMode.Standalone:
                {
                    EnqueueDeferAction(() =>
                    {
                        var e = CreateActor();
                        this.OnPlayerEntered?.Invoke(e);
                    });
                }
                break;
            case WorldMode.Client:
                {
                    var creator = GetFirstCreator();
                    if (null == creator)
                    {
                        throw new NullReferenceException("Player actor creator CAN NOT be null!");
                    }

                    Do(creator.Id, SyncType.Loaded, SyncTarget.Player, 0);
                }
                break;
            }
        }

        public void Do(ulong ownerId, SyncType type, SyncTarget target, ulong actorId, string clsName = null, string subTarget = null, params object[] payload)
        {
            if (WorldMode.Standalone != this.Mode)
            {
                lock (this.syncActions)
                {
                    this.syncActions.Enqueue(new SyncAction
                    {
                        OwnerId = ownerId,
                        Type = type,
                        Target = target,
                        ActorId = actorId,
                        ClsName = clsName,
                        SubTarget = subTarget,
                        Payload = payload,
                    });
                }
            }
        }

        private void Reset()
        {
            if (null != this.socket)
            {
                this.socket.Close();
                this.socket = null;
            }
        }

        private bool IsNetworkValid()
        {
            return (null != this.socket && this.socket.Connected);
        }

        private void Sync(IPeer peer = null)
        {
            Flush(actions =>
            {
                peer?.Send(new Packet(0, actions));
            });
        }

        private void SyncAll()
        {
            Flush(actions =>
            {
                var p = new Packet(0, actions);
                lock (this.peers)
                {
                    foreach (var i in this.peers)
                    {
                        i.Value.Send(p);
                    }
                }
            });
        }

        private void Flush(Action<IEnumerable<SyncAction>> callback)
        {
            if (null != this.socket && this.syncActions.Count > 0)
            {
                lock (this.syncActions)
                {
                    callback(this.syncActions);
                    this.syncActions.Clear();
                }
            }
        }

        private void OnPeerConnected(IPeer peer)
        {
            if (WorldMode.Client == this.Mode)
            {
                Do(this.defaultCreator.Id, SyncType.Authorize, SyncTarget.Player, 0);
                Sync(peer);
            }
            else
            {
                lock (this.peers)
                {
                    this.peers.Add(peer.Id, peer);
                }
            }
        }

        private void OnPeerClosed(IPeer peer, Exception ex)
        {
            switch (this.Mode)
            {
            case WorldMode.Client:
                EnqueueDeferAction(() => { this.OnClosed?.Invoke(ex); });
                break;
            case WorldMode.Server:
                lock (this.syncObject)
                {
                    lock (this.peers)
                    {
                        this.peers.Remove(peer.Id);
                    }

                    EnqueueDeferAction(() =>
                    {
                        this.OnPlayerExited?.Invoke(GetPlayer(peer));
                        DeletePlayer(peer);
                    });
                }
                break;
            }
        }

        private void OnPeerMessage(IPeer peer, object packet)
        {
            var p = (Packet)packet;
            while (p.Actions.Count > 0)
            {
                var a = p.Actions.Dequeue();
                switch (a.Type)
                {
                case SyncType.Authorize:    // Only server handle
                    lock (this.syncObject)
                    {
                        TakeSnapshot();
                        Do(AllocUniqueId(), SyncType.Entered, SyncTarget.Player, 0);
                        Sync(peer);
                    }
                    break;
                case SyncType.Entered:      // Only client handle
                    lock (this.syncObject)
                    {
                        NewPlayer(a.OwnerId, peer);
                        EnqueueDeferAction(() => { this.OnLoading?.Invoke(); });
                    }
                    break;
                case SyncType.Loaded:       // Only server handle
                    lock (this.syncObject)
                    {
                        EnqueueDeferAction(() =>
                        {
                            var e = NewPlayer(a.OwnerId, peer);
                            this.OnPlayerEntered?.Invoke(e);
                        });
                    }
                    break;
                case SyncType.Create:
                    switch (a.Target)
                    {
                    case SyncTarget.Actor:
                        lock (this.syncObject)
                        {
                            CreateActorInternal(GetCreatorById(a.OwnerId), a.ActorId, true, a.SubTarget);
                        }
                        break;
                    case SyncTarget.Trait:
                        lock (this.syncObject)
                        {
                            var e = GetActorById(a.ActorId) as Actor;
                            if (null != e)
                            {
                                e.AddTraitInternal(a.ClsName);
                            }
                        }
                        break;
                    case SyncTarget.Tag:
                        lock (this.syncObject)
                        {
                            var e = GetActorById(a.ActorId) as Actor;
                            if (null != e)
                            {
                                e.AddTagInternal(a.ClsName);
                            }
                        }
                        break;
                    }
                    break;
                case SyncType.Destroy:
                    switch (a.Target)
                    {
                    case SyncTarget.Actor:
                        lock (this.syncObject)
                        {
                            var e = GetActorById(a.ActorId) as Actor;
                            if (null != e)
                            {
                                DestroyActorInternal(e);
                            }
                        }
                        break;
                    case SyncTarget.Trait:
                        lock (this.syncObject)
                        {
                            var e = GetActorById(a.ActorId) as Actor;
                            if (null != e)
                            {
                                e.RemoveTraitInternal(a.ClsName);
                            }
                        }
                        break;
                    case SyncTarget.Tag:
                        lock (this.syncObject)
                        {
                            var e = GetActorById(a.ActorId) as Actor;
                            if (null != e)
                            {
                                e.RemoveTagInternal(a.ClsName);
                            }
                        }
                        break;
                    }
                    break;
                case SyncType.SetProp: // Only apply on Trait
                    lock (this.syncObject)
                    {
                        var e = GetActorById(a.ActorId) as Actor;
                        if (null != e)
                        {
                            var t = e.GetTrait(a.ClsName);
                            t.SetPropInternal(Trait.CallType.Remote, a.OwnerId, a.SubTarget, a.Payload[0]);
                        }
                    }
                    break;
                case SyncType.CallFunc: // Only apply on Trait
                    lock (this.syncObject)
                    {
                        var e = GetActorById(a.ActorId) as Actor;
                        if (null != e)
                        {
                            var t = e.GetTrait(a.ClsName);
                            t.CallFuncInternal(Trait.CallType.Remote, a.OwnerId, a.SubTarget, a.Payload);
                        }
                    }
                    break;
                }
            }
        }

        private IActor NewPlayer(ulong id, IPeer peer)
        {
            var p = new Player(id, this, peer).CreateActor();
            this.players.Add(id, p);

            return p;
        }

        private bool DeletePlayer(IPeer peer)
        {
            foreach (var p in this.players)
            {
                if (((Player)p.Value.Creator).Peer == peer)
                {
                    this.players.Remove(p.Key);
                    var player = (Player)p.Value.Creator;
                    player.Dispose();
                    return true;
                }
            }

            return false;
        }

        private IActor GetPlayer(IPeer peer)
        {
            foreach (var p in this.players)
            {
                if (((Player)p.Value.Creator).Peer == peer)
                {
                    return p.Value;
                }
            }

            return null;
        }

        private ICreator GetCreatorById(ulong id)
        {
            return this.players.TryGetValue(id, out IActor p) ? p.Creator : this.defaultCreator;
        }

        private Creator GetFirstCreator()
        {
            foreach (var p in this.players)
            {
                return (Creator)p.Value.Creator;
            }

            return null;
        }

        private void EnqueueDeferAction(Action action)
        {
            lock (this.deferActions)
            {
                this.deferActions.Enqueue(action);
            }
        }

        private void ExecuteDeferActions()
        {
            lock (this.deferActions)
            {
                while (this.deferActions.Count > 0)
                {
                    var action = this.deferActions.Dequeue();
                    action?.Invoke();
                }
            }
        }

        private struct Packet
        {
            public uint FrameIndex;
            public Queue<SyncAction> Actions;
            public long LocalTimestamp;
            public long RemoteTimestamp;

            public Packet(uint frameIndex)
            {
                this.FrameIndex = frameIndex;
                this.Actions = new Queue<SyncAction>();
                this.LocalTimestamp = DateTime.Now.Ticks;
                this.RemoteTimestamp = 0;
            }

            public Packet(uint frameIndex, IEnumerable<SyncAction> actions)
            {
                this.FrameIndex = frameIndex;
                this.Actions = new Queue<SyncAction>(actions);
                this.LocalTimestamp = DateTime.Now.Ticks;
                this.RemoteTimestamp = 0;
            }

            public bool Marshal(IBufWriter writer)
            {
                //Console.WriteLine("--------------------Marshal------------------------");
                var orgin = writer.Position;
                {
                    writer.WriteInt32(0);
                    writer.WriteUInt32(this.FrameIndex);
                    writer.WriteInt(this.Actions.Count);
                    lock (this.Actions)
                    {
                        while (this.Actions.Count > 0)
                        {
                            var a = this.Actions.Dequeue();

                            //Console.WriteLine("> " + a.OwnerId + ", " + a.Type + ", " + a.Target + ", " + a.ActorId + ", " + (a.ClsName ?? string.Empty) + ", " + (a.SubTarget ?? string.Empty));
                            writer.WriteUInt64(a.OwnerId);
                            writer.WriteByte((byte)a.Type);
                            writer.WriteByte((byte)a.Target);
                            writer.WriteUInt64(a.ActorId);
                            writer.WriteString(a.ClsName ?? string.Empty);
                            writer.WriteString(a.SubTarget ?? string.Empty);
                            writer.WriteByte((byte)a.Payload.Length);
                            for (var i = 0; i < a.Payload.Length; ++i)
                            {
                                WriteObject(writer, a.Payload[i]);
                            }
                        }
                    }

                    writer.WriteInt64(this.LocalTimestamp);
                }
                var length = writer.Position - orgin;
                writer.Seek(orgin);
                writer.WriteInt32(length);

                return true;
            }

            public bool Unmarshal(IBufReader reader)
            {
                //Console.WriteLine("--------------------Unmarshal------------------------");
                var length = reader.ReadInt32();

                this.FrameIndex = reader.ReadUInt32();

                var count = reader.ReadInt32();
                for (var i = 0; i < count; ++i)
                {
                    var ownerId = reader.ReadUInt64();
                    var type = (SyncType)reader.ReadByte();
                    var target = (SyncTarget)reader.ReadByte();
                    var actorId = reader.ReadUInt64();
                    var clsName = reader.ReadString();
                    var subTarget = reader.ReadString();
                    object[] payload = null;
                    var payloadCount = (int)reader.ReadByte();
                    if (payloadCount > 0)
                    {
                        payload = new object[payloadCount];
                        for (var j = 0; j < payloadCount; ++j)
                        {
                            payload[j] = ReadObject(reader);
                        }
                    }

                    //Console.WriteLine("< " + ownerId + ", " + type + ", " + target + ", " + actorId + ", " + (clsName ?? string.Empty) + ", " + (subTarget ?? string.Empty));
                    lock (this.Actions)
                    {
                        this.Actions.Enqueue(new SyncAction
                        {
                            OwnerId = ownerId,
                            Type = type,
                            Target = target,
                            ActorId = actorId,
                            ClsName = string.IsNullOrEmpty(clsName) ? null : clsName,
                            SubTarget = string.IsNullOrEmpty(subTarget) ? null : subTarget,
                            Payload = payload,
                        });
                    }
                }

                this.RemoteTimestamp = reader.ReadInt64();

                return true;
            }
        }

        public static void WriteObject(IBufWriter writer, object value)
        {
            var t = value.GetType();
            if (t.IsValueType)
            {
                if (t == typeof(bool))
                {
                    writer.WriteByte((bool)value ? TRUE : FALSE);
                }
                else if (t.IsPrimitive)
                {
                    if (t == typeof(byte))
                    {
                        writer.WriteByte(INT8);
                        writer.WriteByte((byte)value);
                    }
                    else if (t == typeof(short))
                    {
                        writer.WriteByte(INT16);
                        writer.WriteInt16((short)value);
                    }
                    else if (t == typeof(int))
                    {
                        writer.WriteByte(INT32);
                        writer.WriteInt32((int)value);
                    }
                    else if (t == typeof(long))
                    {
                        writer.WriteByte(INT64);
                        writer.WriteInt64((long)value);
                    }
                    else if (t == typeof(ushort))
                    {
                        writer.WriteByte(UINT16);
                        writer.WriteUInt16((ushort)value);
                    }
                    else if (t == typeof(uint))
                    {
                        writer.WriteByte(UINT32);
                        writer.WriteUInt32((uint)value);
                    }
                    else if (t == typeof(ulong))
                    {
                        writer.WriteByte(UINT64);
                        writer.WriteUInt64((ulong)value);
                    }
                    else if (t == typeof(float))
                    {
                        writer.WriteByte(FLOAT32);
                        writer.WriteFloat((float)value);
                    }
                    else if (t == typeof(double))
                    {
                        writer.WriteByte(FLOAT64);
                        writer.WriteDouble((double)value);
                    }
                }
                else if (t.IsEnum)
                {

                }
                else
                {

                }
            }
            else if (t == typeof(string))
            {
                writer.WriteByte(STR32);
                writer.WriteString((string)value);
            }
            else if (t.IsArray)
            {
                writer.WriteByte(ARR32);
                var arr = value as Array;
                WriteObject(writer, arr.Length);
                for (var i = 0; i < arr.Length; ++i)
                {
                    WriteObject(writer, arr.GetValue(i));
                }
            }
            else if (value is IDictionary)
            {
                writer.WriteByte(MAP32);
                var map = value as IDictionary;
                WriteObject(writer, map.Count);
                var it = map.GetEnumerator();
                while (it.MoveNext())
                {
                    WriteObject(writer, it.Key);
                    WriteObject(writer, it.Value);
                }
            }
            else
            {

            }
        }

        public static object ReadObject(IBufReader reader)
        {
            var flag = reader.ReadByte();
            switch (flag)
            {
            case FALSE:
                return false;
            case TRUE:
                return true;
            case FLOAT32:
                return reader.ReadFloat();
            case FLOAT64:
                return reader.ReadDouble();
            case UINT16:
                return reader.ReadUInt16();
            case UINT32:
                return reader.ReadUInt32();
            case UINT64:
                return reader.ReadUInt64();
            case INT8:
                return reader.ReadByte();
            case INT16:
                return reader.ReadInt16();
            case INT32:
                return reader.ReadInt32();
            case INT64:
                return reader.ReadInt64();
            case STR32:
                return reader.ReadString();
            case ARR32:
                {
                    var count = (int)ReadObject(reader);
                    var arr = new object[count];
                    for (var i = 0; i < count; ++i)
                    {
                        arr[i] = ReadObject(reader);
                    }

                    return arr;
                }
            case MAP32:
                {
                    var count = (int)ReadObject(reader);
                    var map = new Dictionary<object, object>();
                    for (var i = 0; i < count; ++i)
                    {
                        var key = ReadObject(reader);
                        var value = ReadObject(reader);
                        map.Add(key, value);
                    }

                    return map;
                }
            default:
                throw new TypeUnloadedException("DO NOT know the value type!");
            }
        }

        private const byte FALSE = 0xc2;
        private const byte TRUE = 0xc3;
        private const byte FLOAT32 = 0xca;
        private const byte FLOAT64 = 0xcb;
        private const byte UINT8 = 0xcc;
        private const byte UINT16 = 0xcd;
        private const byte UINT32 = 0xce;
        private const byte UINT64 = 0xcf;
        private const byte INT8 = 0xd0;
        private const byte INT16 = 0xd1;
        private const byte INT32 = 0xd2;
        private const byte INT64 = 0xd3;
        private const byte STR32 = 0xdb;
        private const byte ARR32 = 0xdd;
        private const byte MAP32 = 0xdf;
    }
}
