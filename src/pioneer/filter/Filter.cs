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
using System;
using System.Collections.Generic;

namespace Pioneer.Filter
{
    abstract class Filter : IBitCodeTrigger, IDisposable
    {
        private enum TargetActionType
        {
            Init,
            Added,
            Removed,
            Changed,
        }

        private struct TargetAction
        {
            public TargetActionType ActionType;
            public Actor Actor;
            public BitCode Code;
        }

        private bool locked = false;
        private Queue<TargetAction> actions = new Queue<TargetAction>();

        public Filter(Matcher matcher)
        {
            this.Matcher = matcher;
        }

        public Matcher Matcher { get; private set; } = null;

        public virtual void Dispose()
        {
            this.Matcher = null;
        }

        public void OnBitCodeTargetInit(Actor actor)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Init,
                    Actor = actor,
                });
            }
            else
            {
                OnTargetInit(actor);
            }
        }

        public void OnBitCodeTargetAdded(Actor actor, BitCode code)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Added,
                    Actor = actor,
                    Code = code,
                });
            }
            else
            {
                OnTargetAdded(actor, code);
            }
        }

        public void OnBitCodeTargetRemoved(Actor actor, BitCode code)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Removed,
                    Actor = actor,
                    Code = code,
                });
            }
            else
            {
                OnTargetRemoved(actor, code);
            }
        }

        public void OnBitCodeTargetChanged(Actor actor, BitCode code)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Changed,
                    Actor = actor,
                    Code = code,
                });
            }
            else
            {
                OnTargetChanged(actor, code);
            }
        }

        public void OnPreHandling()
        {
            this.locked = true;
        }

        public void OnPostHandling()
        {
            this.locked = false;
            DeferTargetActions();
        }

        protected virtual void OnTargetInit(Actor actor)
        { }

        protected virtual void OnTargetAdded(Actor actor, BitCode code)
        { }

        protected virtual void OnTargetRemoved(Actor actor, BitCode code)
        { }

        protected virtual void OnTargetChanged(Actor actor, BitCode code)
        { }

        protected virtual void OnTargetDeferActions()
        { }

        private void DeferTargetActions()
        {
            OnTargetDeferActions();

            while (this.actions.Count > 0)
            {
                var action = this.actions.Dequeue();
                switch (action.ActionType)
                {
                    case TargetActionType.Init:
                        OnTargetInit(action.Actor);
                        break;
                    case TargetActionType.Added:
                        OnTargetAdded(action.Actor, action.Code);
                        break;
                    case TargetActionType.Removed:
                        OnTargetRemoved(action.Actor, action.Code);
                        break;
                    case TargetActionType.Changed:
                        OnTargetChanged(action.Actor, action.Code);
                        break;
                }
            }
        }
    }
}
