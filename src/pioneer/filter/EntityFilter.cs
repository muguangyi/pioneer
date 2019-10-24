/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Bit;
using Pioneer.Framework;

namespace Pioneer.Filter
{
    abstract class EntityFilter : Filter, IEntityFilter
    {
        protected Entity entity = null;

        public EntityFilter(Matcher matcher) : base(matcher)
        {
            this.entity = null;
        }

        public virtual ITraitContainer Target
        {
            get
            {
                return this.entity;
            }
        }

        public override void Dispose()
        {
            this.entity = null;

            base.Dispose();
        }

        protected virtual void OnEntityMatched(Entity entity)
        {
            this.entity = entity;
        }

        protected virtual void OnEntityUnmatched(Entity entity)
        {
            this.entity = null;
        }

        protected virtual void OnEntityChanged(Entity entity)
        { }

        protected override void OnTargetInit(Entity entity)
        {
            if (entity.CompositeCode.Contains(this.Matcher.MatchCode) &&
                !entity.CompositeCode.Intersect(this.Matcher.ExceptCode))
            {
                OnEntityMatched(entity);
            }
        }

        protected override void OnTargetAdded(Entity entity, BitCode code)
        {
            if (null != this.entity)
            {
                if (this.Matcher.ExceptCode.Contains(code))
                {
                    OnEntityUnmatched(entity);
                }
            }
            else
            {
                if (entity.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !entity.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    OnEntityMatched(entity);
                }
            }
        }

        protected override void OnTargetRemoved(Entity entity, BitCode code)
        {
            if (null != this.entity)
            {
                if (this.Matcher.MatchCode.Contains(code))
                {
                    OnEntityUnmatched(entity);
                }
            }
            else
            {
                if (entity.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !entity.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    OnEntityMatched(entity);
                }
            }
        }

        protected override void OnTargetChanged(Entity entity, BitCode code)
        {
            OnEntityChanged(entity);
        }
    }

    class JobEntityFilter : EntityFilter
    {
        public JobEntityFilter(Matcher matcher) : base(matcher)
        { }
    }

    class ReactEntityFilter : EntityFilter
    {
        public ReactEntityFilter(Matcher matcher) : base(matcher)
        { }

        public override ITraitContainer Target
        {
            get
            {
                return this.entity;
            }
        }

        protected override void OnEntityChanged(Entity entity)
        {
            this.entity = entity;
        }

        protected override void OnTargetDeferActions()
        {
            this.entity = null;
        }
    }
}
