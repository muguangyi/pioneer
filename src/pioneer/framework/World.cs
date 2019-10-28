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
        private LinkedList<IEntity> entities = new LinkedList<IEntity>();
        private Dictionary<ulong, LinkedListNode<IEntity>> entityMap = new Dictionary<ulong, LinkedListNode<IEntity>>();
        private Filters<EntitiesFilter> filters = new Filters<EntitiesFilter>();
        private List<IEntity> cycleEntities = new List<IEntity>();
        private Stack<IEntity> entityPool = new Stack<IEntity>();
        private List<System> systems = new List<System>();
        private BitCodeCenter center = new BitCodeCenter();
        private EntityDecorator decorator = new EntityDecorator();
        private Dictionary<uint, Queue<Trait>> traitPool = new Dictionary<uint, Queue<Trait>>();

        public event Action<IEntity> OnPlayerEntered;
        public event Action<IEntity> OnPlayerExited;

        public World(WorldMode mode = WorldMode.Standalone)
        {
            this.Mode = mode;
            this.filters.Active();
            this.defaultCreator = new EntityCreator(AllocUniqueId(), this);
        }

        public override void Dispose()
        {
            Reset();

            // NOTE: Dispose System first since system dispose logic may
            // need Entity
            for (var i = 0; i < this.systems.Count; ++i)
            {
                this.systems[i].Dispose();
            }

            // NOTE: DONOT dispose trait in traitPool since they have been
            // disposed already when being dropped
            var node = this.entities.First;
            while (null != node)
            {
                (node.Value as Entity).DisposeImmediate();
                node = node.Next;
            }

            while (this.entityPool.Count > 0)
            {
                var e = this.entityPool.Pop() as Entity;
                e.DisposeImmediate();
            }

            this.filters.Dispose();
            this.center.Dispose();

            this.socket = null;
            this.entities = null;
            this.entityMap = null;
            this.filters = null;
            this.cycleEntities = null;
            this.entityPool = null;
            this.center = null;
            this.traitPool = null;
            this.entities = null;
            this.systems = null;

            base.Dispose();
        }

        public IEnumerable<IEntity> Entities
        {
            get
            {
                return this.entities;
            }
        }

        public void Update(float deltaTime)
        {
            BeginFrame(deltaTime);
            lock (this.syncObject)
            {
                var node = this.entities.First;
                while (null != node)
                {
                    (node.Value as Entity).OnUpdate(deltaTime);
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

        public IEntitiesFilter GetFilter(ISystem system, TupleType tupleType, IMatcher matcher)
        {
            var m = matcher as Matcher;

            EntitiesFilter filter = this.filters.GetFilter(system, tupleType, m);
            if (null == filter)
            {
                filter = this.filters.AddFilter
                (
                    system,
                    tupleType,
                    TupleType.Job == tupleType ? new JobEntitiesFilter(m) as EntitiesFilter : new ReactEntitiesFilter(m)
                );

                var node = this.entities.First;
                while (null != node)
                {
                    filter.OnBitCodeTargetInit(node.Value as Entity);
                    node = node.Next;
                }
            }

            return filter;
        }

        public void OnBitCodeTargetInit(Entity target)
        { }

        public void OnBitCodeTargetAdded(Entity target, BitCode code)
        {
            this.filters.OnBitCodeTargetAdded(target, code);
        }

        public void OnBitCodeTargetRemoved(Entity target, BitCode code)
        {
            this.filters.OnBitCodeTargetRemoved(target, code);
        }

        public void OnBitCodeTargetChanged(Entity target, BitCode code)
        {
            this.filters.OnBitCodeTargetChanged(target, code);
        }

        public IEntity CreateEntity(bool replicated = true, string template = null)
        {
            var e = CreateEntityInternal(this.defaultCreator, 0, replicated, template);

            if (replicated && WorldMode.Client != this.Mode)
            {
                this.Do(((EntityCreator)e.Creator).Id, SyncType.Create, SyncTarget.Entity, e.Id, null, template);
            }

            return e;
        }

        public IEntity GetEntityById(ulong entityId)
        {
            LinkedListNode<IEntity> node = null;
            if (this.entityMap.TryGetValue(entityId, out node))
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

        public bool AddEntityTemplate(string template, Action<IEntity> decorator)
        {
            return this.decorator.AddTemplate(template, decorator);
        }

        public override void EndFrame(float deltaTime)
        {
            Entity e = null;
            for (var i = 0; i < this.cycleEntities.Count; ++i)
            {
                e = this.cycleEntities[i] as Entity;
                e.Deactive();

                lock (this.syncObject)
                {
                    LinkedListNode<IEntity> node = null;
                    if (this.entityMap.TryGetValue(e.Id, out node))
                    {
                        this.entityMap.Remove(e.Id);
                        this.entities.Remove(node);
                    }
                }

                this.entityPool.Push(e);
            }
            this.cycleEntities.Clear();

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

        internal Entity CreateEntityByOwner(EntityCreator owner, bool replicated, string template)
        {
            var e = CreateEntityInternal(owner, 0, replicated, template);

            if (replicated && WorldMode.Client != this.Mode)
            {
                this.Do(((EntityCreator)e.Creator).Id, SyncType.Create, SyncTarget.Entity, e.Id, null, template);
            }

            return e;
        }

        internal Entity CreateEntityInternal(IEntityCreator owner, ulong id, bool replicated, string template)
        {
            Entity e = null;
            if (this.entityPool.Count > 0)
            {
                e = this.entityPool.Pop() as Entity;
            }
            else
            {
                e = new Entity(this, this.center);
            }
            e.Replicated = replicated;

            ulong entityId = (0 != id ? id : AllocUniqueId());
            e.Active(owner, entityId, template);
            this.decorator.Apply(template, e);
            this.entityMap.Add(e.Id, this.entities.AddLast(e));

            return e;
        }

        internal void DestroyEntity(Entity entity)
        {
            if (WorldMode.Client != this.Mode)
            {
                this.Do(((EntityCreator)entity.Creator).Id, SyncType.Destroy, SyncTarget.Entity, entity.Id);
            }

            DestroyEntityInternal(entity);
        }

        internal void DestroyEntityInternal(Entity entity)
        {
            this.cycleEntities.Add(entity);
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
            return (flag + (++this.instanceIndex));
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

        private System GetSystem(string systemName)
        {
            return GetSystem(TypeUtility.GetType(systemName));
        }

        private void TakeSnapshot()
        {
            var node = this.entities.First;
            while (null != node)
            {
                var e = node.Value as Entity;
                if (e.Replicated)
                {
                    Do(((EntityCreator)e.Creator).Id, SyncType.Create, SyncTarget.Entity, e.Id, null, e.Template);
                    e.TakeSnapshot();
                }

                node = node.Next;
            }
        }
    }
}
