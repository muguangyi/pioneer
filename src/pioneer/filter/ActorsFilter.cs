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
using System.Collections.Generic;

namespace Pioneer.Filter
{
    abstract class ActorsFilter : Filter, IActorsFilter
    {
        protected LinkedList<IActor> actors = new LinkedList<IActor>();
        protected Dictionary<ulong, LinkedListNode<IActor>> queryTable = new Dictionary<ulong, LinkedListNode<IActor>>();
        private HashSet<ulong> matches = new HashSet<ulong>();

        public ActorsFilter(Matcher matcher) : base(matcher)
        { }

        public int Count => this.actors.Count;

        public IEnumerable<IActor> Target => this.actors;

        public IActor First
        {
            get
            {
                var first = this.actors.First;
                return first?.Value;
            }
        }

        public override void Dispose()
        {
            this.matches = null;

            base.Dispose();
        }

        protected virtual void OnActorMatched(Actor actor)
        {
            if (!this.queryTable.TryGetValue(actor.Id, out LinkedListNode<IActor> node))
            {
                node = this.actors.AddLast(actor);
                this.queryTable.Add(actor.Id, node);
            }
        }

        protected virtual void OnActorUnmatched(Actor actor)
        {
            if (this.queryTable.TryGetValue(actor.Id, out LinkedListNode<IActor> node))
            {
                this.actors.Remove(node);
                this.queryTable.Remove(actor.Id);
            }
        }

        protected virtual void OnActorChanged(Actor actor)
        { }

        protected override void OnTargetInit(Actor actor)
        {
            if (actor.CompositeCode.Contains(this.Matcher.MatchCode) &&
                !actor.CompositeCode.Intersect(this.Matcher.ExceptCode))
            {
                this.matches.Add(actor.Id);
                OnActorMatched(actor);
            }
        }

        protected override void OnTargetAdded(Actor actor, BitCode code)
        {
            if (this.matches.Contains(actor.Id))
            {
                if (this.Matcher.ExceptCode.Contains(code))
                {
                    this.matches.Remove(actor.Id);
                    OnActorUnmatched(actor);
                }
            }
            else
            {
                if (actor.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !actor.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    this.matches.Add(actor.Id);
                    OnActorMatched(actor);
                }
            }
        }

        protected override void OnTargetRemoved(Actor actor, BitCode code)
        {
            if (this.matches.Contains(actor.Id))
            {
                if (this.Matcher.MatchCode.Contains(code))
                {
                    this.matches.Remove(actor.Id);
                    OnActorUnmatched(actor);
                }
            }
            else
            {
                if (actor.CompositeCode.Contains(this.Matcher.MatchCode) &&
                    !actor.CompositeCode.Intersect(this.Matcher.ExceptCode))
                {
                    this.matches.Add(actor.Id);
                    OnActorMatched(actor);
                }
            }
        }

        protected override void OnTargetChanged(Actor actor, BitCode code)
        {
            if (this.matches.Contains(actor.Id))
            {
                OnActorChanged(actor);
            }
        }
    }

    class JobActorsFilter : ActorsFilter
    {
        public JobActorsFilter(Matcher matcher) : base(matcher)
        { }
    }

    class ReactActorsFilter : ActorsFilter
    {
        public ReactActorsFilter(Matcher matcher) : base(matcher)
        { }

        protected override void OnActorChanged(Actor actor)
        {
            if (!this.queryTable.TryGetValue(actor.Id, out LinkedListNode<IActor> node))
            {
                node = this.actors.AddLast(actor);
                this.queryTable.Add(actor.Id, node);
            }
        }

        protected override void OnTargetDeferActions()
        {
            this.actors.Clear();
            this.queryTable.Clear();
        }
    }
}
