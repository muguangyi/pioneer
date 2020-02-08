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
    abstract class ActorFilter : Filter, IFilter
    {
        protected Actor actor = null;

        public ActorFilter(Matcher matcher) : base(matcher)
        {
            this.actor = null;
        }

        public virtual bool Matched => this.actor != null;

        public override void Dispose()
        {
            this.actor = null;

            base.Dispose();
        }

        protected virtual void OnActorMatched(Actor actor)
        {
            this.actor = actor;
        }

        protected virtual void OnActorUnmatched(Actor actor)
        {
            this.actor = null;
        }

        protected virtual void OnActorChanged(Actor actor)
        { }

        protected override void OnTargetInit(Actor actor)
        {
            if (actor.CompositeCode.Contains(this.Matcher.MatchCode) &&
                !actor.CompositeCode.Intersect(this.Matcher.ExceptCode))
            {
                OnActorMatched(actor);
            }
        }

        protected override void OnTargetAdded(Actor actor, BitCode code)
        {
            if (null != this.actor)
            {
                if (this.Matcher.ExceptCode.Contains(code))
                {
                    OnActorUnmatched(actor);
                }
            }
            else
            {
                if (actor.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !actor.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    OnActorMatched(actor);
                }
            }
        }

        protected override void OnTargetRemoved(Actor actor, BitCode code)
        {
            if (null != this.actor)
            {
                if (this.Matcher.MatchCode.Contains(code))
                {
                    OnActorUnmatched(actor);
                }
            }
            else
            {
                if (actor.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !actor.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    OnActorMatched(actor);
                }
            }
        }

        protected override void OnTargetChanged(Actor actor, BitCode code)
        {
            OnActorChanged(actor);
        }
    }

    class JobActorFilter : ActorFilter
    {
        public JobActorFilter(Matcher matcher) : base(matcher)
        { }
    }

    class ReactActorFilter : ActorFilter
    {
        public ReactActorFilter(Matcher matcher) : base(matcher)
        { }

        protected override void OnActorChanged(Actor actor)
        {
            this.actor = actor;
        }

        protected override void OnTargetDeferActions()
        {
            this.actor = null;
        }
    }
}
