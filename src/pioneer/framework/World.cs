/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Base;
using Pioneer.Bit;
using Pioneer.Filter;
using Pioneer.Framework;
using Pioneer.Sync;
using Pioneer.Util;
using System;
using System.Collections.Generic;

namespace Pioneer
{
    sealed partial class World : FrameObject, IWorld
    {
        private ulong instanceIndex = 0;
        private LinkedList<IActor> actors = new LinkedList<IActor>();
        private Dictionary<ulong, LinkedListNode<IActor>> actorMap = new Dictionary<ulong, LinkedListNode<IActor>>();
        private Filters<ActorsFilter> filters = new Filters<ActorsFilter>();
        private List<IActor> cycleActors = new List<IActor>();
        private Stack<IActor> actorPool = new Stack<IActor>();
        private List<System> systems = new List<System>();
        private BitCodeCenter center = new BitCodeCenter();
        private ActorDecorator decorator = new ActorDecorator();
        private Dictionary<uint, Queue<Trait>> traitPool = new Dictionary<uint, Queue<Trait>>();

        public event Action<IActor> OnPlayerEntered;
        public event Action<IActor> OnPlayerExited;

        public World(WorldMode mode = WorldMode.Standalone)
        {
            this.Mode = mode;
            this.filters.Active();
            this.defaultCreator = new Creator(AllocUniqueId(), this);
        }

        public override void Dispose()
        {
            Reset();

            // NOTE: Dispose System first since system dispose logic may
            // need Actor
            for (var i = 0; i < this.systems.Count; ++i)
            {
                this.systems[i].Dispose();
            }

            // NOTE: DONOT dispose trait in traitPool since they have been
            // disposed already when being dropped
            var node = this.actors.First;
            while (null != node)
            {
                (node.Value as Actor).DisposeImmediate();
                node = node.Next;
            }

            while (this.actorPool.Count > 0)
            {
                var e = this.actorPool.Pop() as Actor;
                e.DisposeImmediate();
            }

            this.filters.Dispose();
            this.center.Dispose();

            this.socket = null;
            this.actors = null;
            this.actorMap = null;
            this.filters = null;
            this.cycleActors = null;
            this.actorPool = null;
            this.center = null;
            this.traitPool = null;
            this.actors = null;
            this.systems = null;

            base.Dispose();
        }

        public IEnumerable<IActor> Actors => this.actors;

        public void Update(float deltaTime)
        {
            BeginFrame(deltaTime);
            lock (this.syncObject)
            {
                var node = this.actors.First;
                while (null != node)
                {
                    (node.Value as Actor).OnUpdate(deltaTime);
                    node = node.Next;
                }

                System s = null;
                for (var i = 0; i < this.systems.Count; ++i)
                {
                    s = this.systems[i];
                    this.filters.OnPreHandling(s);
                    {
                        s.OnUpdate(this, deltaTime);
                    }
                    this.filters.OnPostHandling(s);
                }
            }
            EndFrame(deltaTime);
        }

        public IMatcher NewMatcher()
        {
            return new Matcher(this.center);
        }

        public IGroupFilter GetFilter(ISystem system, IMatcher matcher, TupleType tupleType = TupleType.Job)
        {
            var m = matcher as Matcher;

            ActorsFilter filter = this.filters.GetFilter(system, tupleType, m);
            if (null == filter)
            {
                filter = this.filters.AddFilter
                (
                    system,
                    tupleType,
                    TupleType.Job == tupleType ? new JobActorsFilter(m) as ActorsFilter : new ReactActorsFilter(m)
                );

                var node = this.actors.First;
                while (null != node)
                {
                    filter.OnBitCodeTargetInit(node.Value as Actor);
                    node = node.Next;
                }
            }

            return filter;
        }

        public void OnBitCodeTargetInit(Actor target)
        { }

        public void OnBitCodeTargetAdded(Actor target, BitCode code)
        {
            this.filters.OnBitCodeTargetAdded(target, code);
        }

        public void OnBitCodeTargetRemoved(Actor target, BitCode code)
        {
            this.filters.OnBitCodeTargetRemoved(target, code);
        }

        public void OnBitCodeTargetChanged(Actor target, BitCode code)
        {
            this.filters.OnBitCodeTargetChanged(target, code);
        }

        public IActor CreateActor(bool replicated = true, string template = null)
        {
            var e = CreateActorInternal(this.defaultCreator, 0, replicated, template);

            if (replicated && WorldMode.Client != this.Mode)
            {
                this.Do(((Creator)e.Creator).Id, SyncType.Create, SyncTarget.Actor, e.Id, null, template);
            }

            return e;
        }

        public IActor GetActorById(ulong actorId)
        {
            LinkedListNode<IActor> node = null;
            if (this.actorMap.TryGetValue(actorId, out node))
            {
                return node.Value;
            }

            return null;
        }

        public System AddSystem(Type systemType)
        {
            return AddSystemInternal(systemType);
        }

        public TSystem AddSystem<TSystem>() where TSystem : System
        {
            var system = AddSystem(typeof(TSystem));
            return (null != system ? (TSystem)system : default(TSystem));
        }

        public System AddSystem(string systemName)
        {
            return AddSystem(TypeUtility.GetType(systemName));
        }

        public bool TrySetActorTemplate(string template, Action<IActor> decorator)
        {
            return this.decorator.TrySetTemplate(template, decorator);
        }

        public override void EndFrame(float deltaTime)
        {
            Actor e = null;
            for (var i = 0; i < this.cycleActors.Count; ++i)
            {
                e = this.cycleActors[i] as Actor;
                e.Deactive();

                lock (this.syncObject)
                {
                    LinkedListNode<IActor> node = null;
                    if (this.actorMap.TryGetValue(e.Id, out node))
                    {
                        this.actorMap.Remove(e.Id);
                        this.actors.Remove(node);
                    }
                }

                this.actorPool.Push(e);
            }
            this.cycleActors.Clear();

            ExecuteDeferActions();

            switch (this.Mode)
            {
            case WorldMode.Client:
                if (IsNetworkValid())
                {
                    Sync();
                }
                break;
            case WorldMode.Server:
                if (IsNetworkValid())
                {
                    SyncAll();
                }
                break;
            }
        }

        internal Actor CreateActorByOwner(Creator owner, bool replicated, string template)
        {
            var e = CreateActorInternal(owner, 0, replicated, template);

            if (replicated && WorldMode.Client != this.Mode)
            {
                this.Do(((Creator)e.Creator).Id, SyncType.Create, SyncTarget.Actor, e.Id, null, template);
            }

            return e;
        }

        internal Actor CreateActorInternal(ICreator owner, ulong id, bool replicated, string template)
        {
            Actor a = null;
            if (this.actorPool.Count > 0)
            {
                a = this.actorPool.Pop() as Actor;
            }
            else
            {
                a = new Actor(this, this.center);
            }
            a.Replicated = replicated;

            ulong actorId = (0 != id ? id : AllocUniqueId());
            a.Active(owner, actorId, template);
            this.decorator.Apply(template, a);
            this.actorMap.Add(a.Id, this.actors.AddLast(a));

            return a;
        }

        internal void DestroyActor(Actor actor)
        {
            if (WorldMode.Client != this.Mode)
            {
                this.Do(((Creator)actor.Creator).Id, SyncType.Destroy, SyncTarget.Actor, actor.Id);
            }

            DestroyActorInternal(actor);
        }

        internal void DestroyActorInternal(Actor actor)
        {
            this.cycleActors.Add(actor);
        }

        internal System AddSystemInternal(Type systemType)
        {
            if (null == GetSystem(systemType) && typeof(System).IsAssignableFrom(systemType))
            {
                var system = Activator.CreateInstance(systemType) as System;
                if (system.IsApplied(this.Mode))
                {
                    system.OnInit(this);
                    this.systems.Add(system);
                }

                return system;
            }

            return null;
        }

        internal TSystem AddSystemInternal<TSystem>() where TSystem : System
        {
            var system = AddSystemInternal(typeof(TSystem));
            return (null != system ? (TSystem)system : default(TSystem));
        }

        internal System AddSystemInternal(string systemName)
        {
            return AddSystemInternal(TypeUtility.GetType(systemName));
        }

        internal Trait PickTrait(Type componentType)
        {
            var code = this.center.GetBitCodeByType(componentType);
            Queue<Trait> list = null;
            if (!this.traitPool.TryGetValue(code.Index, out list))
            {
                this.traitPool.Add(code.Index, list = new Queue<Trait>());
            }

            Trait instance = null;
            if (0 == list.Count)
            {
                instance = Activator.CreateInstance(componentType) as Trait;
            }
            else
            {
                instance = list.Dequeue();
            }

            return instance;
        }

        internal void DropTrait(Trait instance)
        {
            instance.Dispose();

            var code = this.center.GetBitCodeByType(instance.GetType());
            Queue<Trait> list = null;
            if (!this.traitPool.TryGetValue(code.Index, out list))
            {
                this.traitPool.Add(code.Index, list = new Queue<Trait>());
            }

            list.Enqueue(instance);
        }

        private ulong AllocUniqueId()
        {
            ulong flag = (WorldMode.Client == this.Mode ? (ulong)0 : (uint)1 << 63);
            return flag + (++this.instanceIndex);
        }

        private System GetSystem(Type systemType)
        {
            for (var i = 0; i < this.systems.Count; ++i)
            {
                if (this.systems[i].GetType() == systemType)
                {
                    return this.systems[i];
                }
            }

            return null;
        }

        private void TakeSnapshot()
        {
            var node = this.actors.First;
            while (null != node)
            {
                var e = node.Value as Actor;
                if (e.Replicated)
                {
                    Do(((Creator)e.Creator).Id, SyncType.Create, SyncTarget.Actor, e.Id, null, e.Template);
                    e.TakeSnapshot();
                }

                node = node.Next;
            }
        }
    }
}
