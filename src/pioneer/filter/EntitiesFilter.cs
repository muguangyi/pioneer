/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;

namespace Pioneer
{
    abstract class EntitiesFilter : Filter, IEntitiesFilter
    {
        protected LinkedList<IEntity> entities = new LinkedList<IEntity>();
        protected Dictionary<ulong, LinkedListNode<IEntity>> queryTable = new Dictionary<ulong, LinkedListNode<IEntity>>();
        private HashSet<ulong> matches = new HashSet<ulong>();

        public EntitiesFilter(Matcher matcher) : base(matcher)
        { }

        public int Count
        {
            get
            {
                return this.entities.Count;
            }
        }

        public IEnumerable<IEntity> Target
        {
            get
            {
                return this.entities;
            }
        }

        public IEntity First
        {
            get
            {
                var first = this.entities.First;
                return (null != first ? first.Value : null);
            }
        }

        public override void Dispose()
        {
            this.matches = null;

            base.Dispose();
        }

        protected virtual void OnEntityMatched(Entity entity)
        {
            if (!this.queryTable.TryGetValue(entity.Id, out LinkedListNode<IEntity> node))
            {
                node = this.entities.AddLast(entity);
                this.queryTable.Add(entity.Id, node);
            }
        }

        protected virtual void OnEntityUnmatched(Entity entity)
        {
            if (this.queryTable.TryGetValue(entity.Id, out LinkedListNode<IEntity> node))
            {
                this.entities.Remove(node);
                this.queryTable.Remove(entity.Id);
            }
        }

        protected virtual void OnEntityChanged(Entity entity)
        { }

        protected override void OnTargetInit(Entity entity)
        {
            if (entity.CompositeCode.Contains(this.Matcher.MatchCode) &&
                !entity.CompositeCode.Intersect(this.Matcher.ExceptCode))
            {
                this.matches.Add(entity.Id);
                OnEntityMatched(entity);
            }
        }

        protected override void OnTargetAdded(Entity entity, BitCode code)
        {
            if (this.matches.Contains(entity.Id))
            {
                if (this.Matcher.ExceptCode.Contains(code))
                {
                    this.matches.Remove(entity.Id);
                    OnEntityUnmatched(entity);
                }
            }
            else
            {
                if (entity.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !entity.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    this.matches.Add(entity.Id);
                    OnEntityMatched(entity);
                }
            }
        }

        protected override void OnTargetRemoved(Entity entity, BitCode code)
        {
            if (this.matches.Contains(entity.Id))
            {
                if (this.Matcher.MatchCode.Contains(code))
                {
                    this.matches.Remove(entity.Id);
                    OnEntityUnmatched(entity);
                }
            }
            else
            {
                if (entity.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !entity.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    this.matches.Add(entity.Id);
                    OnEntityMatched(entity);
                }
            }
        }

        protected override void OnTargetChanged(Entity entity, BitCode code)
        {
            if (this.matches.Contains(entity.Id))
            {
                OnEntityChanged(entity);
            }
        }
    }

    class JobEntitiesFilter : EntitiesFilter
    {
        public JobEntitiesFilter(Matcher matcher) : base(matcher)
        { }
    }

    class ReactEntitiesFilter : EntitiesFilter
    {
        public ReactEntitiesFilter(Matcher matcher) : base(matcher)
        { }

        protected override void OnEntityChanged(Entity entity)
        {
            LinkedListNode<IEntity> node = null;
            if (!this.queryTable.TryGetValue(entity.Id, out node))
            {
                node = this.entities.AddLast(entity);
                this.queryTable.Add(entity.Id, node);
            }
        }

        protected override void OnTargetDeferActions()
        {
            this.entities.Clear();
            this.queryTable.Clear();
        }
    }
}
