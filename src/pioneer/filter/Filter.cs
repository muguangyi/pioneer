/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Pioneer
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
            public Entity Entity;
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

        public void OnBitCodeTargetInit(Entity entity)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Init,
                    Entity = entity,
                });
            }
            else
            {
                OnTargetInit(entity);
            }
        }

        public void OnBitCodeTargetAdded(Entity entity, BitCode code)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Added,
                    Entity = entity,
                    Code = code,
                });
            }
            else
            {
                OnTargetAdded(entity, code);
            }
        }

        public void OnBitCodeTargetRemoved(Entity entity, BitCode code)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Removed,
                    Entity = entity,
                    Code = code,
                });
            }
            else
            {
                OnTargetRemoved(entity, code);
            }
        }

        public void OnBitCodeTargetChanged(Entity entity, BitCode code)
        {
            if (this.locked)
            {
                this.actions.Enqueue(new TargetAction
                {
                    ActionType = TargetActionType.Changed,
                    Entity = entity,
                    Code = code,
                });
            }
            else
            {
                OnTargetChanged(entity, code);
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

        protected virtual void OnTargetInit(Entity entity)
        { }

        protected virtual void OnTargetAdded(Entity entity, BitCode code)
        { }

        protected virtual void OnTargetRemoved(Entity entity, BitCode code)
        { }

        protected virtual void OnTargetChanged(Entity entity, BitCode code)
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
                    OnTargetInit(action.Entity);
                    break;
                case TargetActionType.Added:
                    OnTargetAdded(action.Entity, action.Code);
                    break;
                case TargetActionType.Removed:
                    OnTargetRemoved(action.Entity, action.Code);
                    break;
                case TargetActionType.Changed:
                    OnTargetChanged(action.Entity, action.Code);
                    break;
                }
            }
        }
    }
}
