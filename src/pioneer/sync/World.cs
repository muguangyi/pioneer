/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Pioneer
{
    sealed partial class World
    {
        public event Action OnLoading;
        public event Action<Exception> OnClosed;

        public void Start(string nsp = null)
        {
            if (GameMode.Standalone == this.GameMode)
            {
                EnqueueDeferAction(() => { this.OnLoading?.Invoke(); });
            }
            else
            {
                if (null == nsp)
                {
                    throw new ArgumentNullException(string.Format("nsp should not be NULL under {0} mode!", this.GameMode));
                }

                //this.beatState = this.beatState ?? new HeartBeatState(this);

                Reset();

                //this.network = GBox.Make<INetworkManager>().Create("pioneer.net", nsp);
                //this.network.OnAccepted += OnChannelAccepted;
                //this.network.OnConnected += OnChannelConnected;
                //this.network.OnClosed += OnChannelClosed;
                //this.network.OnError += OnChannelError;
                //this.network.OnPacket += OnChannelPacket;
                //this.network.SetPlugins(this);

                switch (this.GameMode)
                {
                case GameMode.Client:
                    //this.network.Connect();
                    break;
                case GameMode.Server:
                    //this.network.SetPlugins(this.beatState);
                    //this.network.Accept();
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
            switch (this.GameMode)
            {
            case GameMode.Standalone:
                {
                    EnqueueDeferAction(() =>
                    {
                        var e = CreateEntity();
                        this.OnPlayerEntered?.Invoke(e);
                    });
                }
                break;
            case GameMode.Client:
                {
                    var creator = GetFirstEntityCreator();
                    if (null == creator)
                    {
                        throw new NullReferenceException("Player entity creator CAN NOT be null!");
                    }

                    Do(creator.Id, SyncType.Loaded, SyncTarget.Player, 0);
                }
                break;
            }
        }

        public ArraySegment<byte> Pack(object packet)
        {
            var p = (Packet)packet;
            return p.Marshal(this.marshalBuffer);
        }

        public object Unpack(ArraySegment<byte> packet)
        {
            return new Packet(0).Unmarshal(packet);
        }

        public void Do(ulong ownerId, SyncType type, SyncTarget target, ulong entityId, string clsName = null, string subTarget = null, params object[] payload)
        {
            if (GameMode.Standalone != this.GameMode)
            {
                lock (this.syncActions)
                {
                    this.syncActions.Enqueue(new SyncAction
                    {
                        OwnerId = ownerId,
                        Type = type,
                        Target = target,
                        EntityId = entityId,
                        ClsName = clsName,
                        SubTarget = subTarget,
                        Payload = payload,
                    });
                }
            }
        }

        private void Reset()
        {
            if (null != this.network)
            {
                //this.network.Disconnect();
                this.network = null;
            }
        }

        private bool IsNetworkValid()
        {
            return (null != this.network && this.network.Connected);
        }

        private void Sync(IPeer targetChannel = null)
        {
            Flush(actions =>
            {
                //this.network.SendTo(new Packet(0, actions), targetChannel);
            });
        }

        private void SyncAll(params IPeer[] exceptChannels)
        {
            Flush(actions =>
            {
                //this.network.SendToAll(new Packet(0, actions), exceptChannels);
            });
        }

        private void Flush(Action<IEnumerable<SyncAction>> callback)
        {
            if (null != this.network && this.syncActions.Count > 0)
            {
                lock (this.syncActions)
                {
                    callback(this.syncActions);
                    this.syncActions.Clear();
                }
            }
        }

        private void OnChannelAccepted(ISocket network, IPeer peer)
        {

        }

        private void OnChannelConnected(ISocket network, IPeer peer)
        {
            if (GameMode.Client == this.GameMode)
            {
                Do(this.defaultCreator.Id, SyncType.Authorize, SyncTarget.Player, 0);
                Sync(peer);
            }
        }

        private void OnChannelClosed(ISocket network, IPeer peer, Exception ex)
        {
            switch (this.GameMode)
            {
            case GameMode.Client:
                EnqueueDeferAction(() => { this.OnClosed?.Invoke(ex); });
                break;
            case GameMode.Server:
                lock (this.syncObject)
                {
                    EnqueueDeferAction(() =>
                    {
                        this.OnPlayerExited?.Invoke(GetPlayer(peer));
                        DeletePlayer(peer);
                    });
                }
                break;
            }
        }

        private void OnChannelError(ISocket network, IPeer peer, Exception ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }

        private void OnChannelPacket(ISocket network, IPeer peer, object packet)
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
                    case SyncTarget.Entity:
                        lock (this.syncObject)
                        {
                            CreateEntityInternal(GetEntityCreatorById(a.OwnerId), a.EntityId, true, a.SubTarget);
                        }
                        break;
                    case SyncTarget.Trait:
                        lock (this.syncObject)
                        {
                            var e = GetEntityById(a.EntityId) as Entity;
                            if (null != e)
                            {
                                e.AddTraitInternal(a.ClsName);
                            }
                        }
                        break;
                    case SyncTarget.Tag:
                        lock (this.syncObject)
                        {
                            var e = GetEntityById(a.EntityId) as Entity;
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
                    case SyncTarget.Entity:
                        lock (this.syncObject)
                        {
                            var e = GetEntityById(a.EntityId) as Entity;
                            if (null != e)
                            {
                                DestroyEntityInternal(e);
                            }
                        }
                        break;
                    case SyncTarget.Trait:
                        lock (this.syncObject)
                        {
                            var e = GetEntityById(a.EntityId) as Entity;
                            if (null != e)
                            {
                                e.RemoveTraitInternal(a.ClsName);
                            }
                        }
                        break;
                    case SyncTarget.Tag:
                        lock (this.syncObject)
                        {
                            var e = GetEntityById(a.EntityId) as Entity;
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
                        var e = GetEntityById(a.EntityId) as Entity;
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
                        var e = GetEntityById(a.EntityId) as Entity;
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

        private IEntity NewPlayer(ulong id, IPeer peer)
        {
            var p = new Player(id, this, peer).CreateEntity();
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

        private IEntity GetPlayer(IPeer peer)
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

        private IEntityCreator GetEntityCreatorById(ulong id)
        {
            IEntity p = null;
            return (this.players.TryGetValue(id, out p) ? p.Creator : this.defaultCreator);
        }

        private EntityCreator GetFirstEntityCreator()
        {
            foreach (var p in this.players)
            {
                return (EntityCreator)p.Value.Creator;
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

            public ArraySegment<byte> Marshal(byte[] buffer)
            {
                //Console.WriteLine("--------------------Marshal------------------------");
                int length = 0;
                using (var stream = new MemoryStream(buffer))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(0);
                        writer.Write(this.FrameIndex);
                        writer.Write(this.Actions.Count);
                        lock (this.Actions)
                        {
                            while (this.Actions.Count > 0)
                            {
                                var a = this.Actions.Dequeue();

                                //Console.WriteLine("> " + a.OwnerId + ", " + a.Type + ", " + a.Target + ", " + a.EntityId + ", " + (a.ClsName ?? string.Empty) + ", " + (a.SubTarget ?? string.Empty));
                                writer.Write(a.OwnerId);
                                writer.Write((byte)a.Type);
                                writer.Write((byte)a.Target);
                                writer.Write(a.EntityId);
                                writer.Write(a.ClsName ?? string.Empty);
                                writer.Write(a.SubTarget ?? string.Empty);
                                writer.Write((byte)a.Payload.Length);
                                for (var i = 0; i < a.Payload.Length; ++i)
                                {
                                    WriteObject(writer, a.Payload[i]);
                                }
                            }
                        }

                        writer.Write(this.LocalTimestamp);

                        length = (int)stream.Position;
                    }
                }

                using (var stream = new MemoryStream(buffer))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(length);
                    }
                }

                return new ArraySegment<byte>(buffer, 0, length + 4);
            }

            public Packet Unmarshal(ArraySegment<byte> data)
            {
                //Console.WriteLine("--------------------Unmarshal------------------------");
                using (var stream = new MemoryStream(data.Array, data.Offset, data.Count))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var length = reader.ReadInt32();

                        this.FrameIndex = reader.ReadUInt32();

                        var count = reader.ReadInt32();
                        for (var i = 0; i < count; ++i)
                        {
                            var ownerId = reader.ReadUInt64();
                            var type = (SyncType)reader.ReadByte();
                            var target = (SyncTarget)reader.ReadByte();
                            var entityId = reader.ReadUInt64();
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

                            //Console.WriteLine("< " + ownerId + ", " + type + ", " + target + ", " + entityId + ", " + (clsName ?? string.Empty) + ", " + (subTarget ?? string.Empty));
                            lock (this.Actions)
                            {
                                this.Actions.Enqueue(new SyncAction
                                {
                                    OwnerId = ownerId,
                                    Type = type,
                                    Target = target,
                                    EntityId = entityId,
                                    ClsName = string.IsNullOrEmpty(clsName) ? null : clsName,
                                    SubTarget = string.IsNullOrEmpty(subTarget) ? null : subTarget,
                                    Payload = payload,
                                });
                            }
                        }

                        this.RemoteTimestamp = reader.ReadInt64();

                        return this;
                    }
                }
            }
        }

        public GameMode GameMode { get; private set; }

        private EntityCreator defaultCreator = null;
        private Dictionary<ulong, IEntity> players = new Dictionary<ulong, IEntity>();
        private Queue<SyncAction> syncActions = new Queue<SyncAction>();
        private Queue<Action> deferActions = new Queue<Action>();
        private ISocket network = null;
        private byte[] marshalBuffer = new byte[BufferSize];
        private object syncObject = new object();
        //private HeartBeatState beatState = null;

        public static void WriteObject(BinaryWriter writer, object value)
        {
            var t = value.GetType();
            if (t.IsValueType)
            {
                if (t == typeof(bool))
                {
                    writer.Write((bool)value ? TRUE : FALSE);
                }
                else if (t.IsPrimitive)
                {
                    if (t == typeof(byte))
                    {
                        writer.Write(INT8);
                        writer.Write((byte)value);
                    }
                    else if (t == typeof(short))
                    {
                        writer.Write(INT16);
                        writer.Write((short)value);
                    }
                    else if (t == typeof(int))
                    {
                        writer.Write(INT32);
                        writer.Write((int)value);
                    }
                    else if (t == typeof(long))
                    {
                        writer.Write(INT64);
                        writer.Write((long)value);
                    }
                    else if (t == typeof(ushort))
                    {
                        writer.Write(UINT16);
                        writer.Write((ushort)value);
                    }
                    else if (t == typeof(uint))
                    {
                        writer.Write(UINT32);
                        writer.Write((uint)value);
                    }
                    else if (t == typeof(ulong))
                    {
                        writer.Write(UINT64);
                        writer.Write((ulong)value);
                    }
                    else if (t == typeof(float))
                    {
                        writer.Write(FLOAT32);
                        writer.Write((float)value);
                    }
                    else if (t == typeof(double))
                    {
                        writer.Write(FLOAT64);
                        writer.Write((double)value);
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
                writer.Write(STR32);
                writer.Write((string)value);
            }
            else if (t.IsArray)
            {
                writer.Write(ARR32);
                var arr = value as Array;
                WriteObject(writer, arr.Length);
                for (var i = 0; i < arr.Length; ++i)
                {
                    WriteObject(writer, arr.GetValue(i));
                }
            }
            else if (value is IDictionary)
            {
                writer.Write(MAP32);
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

        public static object ReadObject(BinaryReader reader)
        {
            var flag = reader.ReadByte();
            switch (flag)
            {
            case FALSE:
                return false;
            case TRUE:
                return true;
            case FLOAT32:
                return reader.ReadSingle();
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

        private const int BufferSize = 1024 * 256;

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
