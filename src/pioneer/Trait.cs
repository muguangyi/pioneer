/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Framework;
using Pioneer.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pioneer
{
    public abstract class Trait : ITrait
    {
        private readonly string typeName = null;
        private Dictionary<string, AbstractMeta> props = new Dictionary<string, AbstractMeta>();
        private Dictionary<string, AbstractArgs> funcs = new Dictionary<string, AbstractArgs>();

        public Trait()
        {
            var type = GetType();
            this.typeName = type.FullName;

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                                 .Where(p => p.GetCustomAttributes(typeof(PPropAttribute), false).Length > 0);
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttributes(typeof(PPropAttribute), false)[0] as PPropAttribute;
                var proxy = (AbstractMeta)Activator.CreateInstance
                (
                    attr.Format,
                    attr.DefaultValue,
                    property,
                    this,
                    attr.Domain,
                    attr.Condition
                );
                this.props.Add(property.Name, proxy);
            }

            var functions = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                                .Where(m => m.GetCustomAttributes(typeof(PFuncAttribute), true).Length > 0);
            foreach (var function in functions)
            {
                var attr = function.GetCustomAttributes(typeof(PFuncAttribute), false)[0] as PFuncAttribute;
                var proxy = (AbstractArgs)Activator.CreateInstance
                (
                    attr.Format,
                    Delegate.CreateDelegate(GetMethodType(attr.Format), this, function),
                    attr.Domain,
                    attr.Condition
                );
                this.funcs.Add(function.Name, proxy);
            }
        }

        internal Entity Entity { private get; set; }

        /// <inheritdoc />
        public event Action<ITrait> OnChanged;

        /// <inheritdoc />
        public virtual void Dispose()
        {
            // NOTE: DO NOT clear props and funcs since Trait is always reused!
            // Only reset properties when Dispose
            foreach (var p in this.props)
            {
                p.Value.Dispose();
            }
        }

        /// <inheritdoc />
        public virtual void OnInit()
        { }

        public void SetProp(string name, object value)
        {
            if (((EntityCreator)this.Entity.Creator).HasAuthority)
            {
                SetPropInternal(CallType.Local, ((EntityCreator)this.Entity.Creator).Id, name, value);
            }
        }

        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public void CallFunc(string name, params object[] args)
        {
            if (((EntityCreator)this.Entity.Creator).HasAuthority)
            {
                CallFuncInternal(CallType.Local, ((EntityCreator)this.Entity.Creator).Id, name, args);
            }
        }

        /// <summary>
        /// 数据改变通知
        /// </summary>
        protected void NotifyChanged()
        {
            this.OnChanged?.Invoke(this);
        }

        internal enum CallType
        {
            Local,
            Remote,
        }

        internal void SetPropInternal(CallType callType, ulong ownerId, string name, object value)
        {
            AbstractMeta proxy;
            if (this.props.TryGetValue(name, out proxy))
            {
                Invoke(proxy, callType, SyncType.SetProp, ownerId, name, value);
            }
        }

        internal void CallFuncInternal(CallType callType, ulong ownerId, string name, params object[] args)
        {
            AbstractArgs proxy;
            if (this.funcs.TryGetValue(name, out proxy))
            {
                Invoke(proxy, callType, SyncType.CallFunc, ownerId, name, args);
            }
        }

        internal void TakeSnapshot()
        {
            var owner = (EntityCreator)this.Entity.Creator;
            var world = owner.World;
            foreach (var i in this.props)
            {
                world.Do(owner.Id, SyncType.SetProp, SyncTarget.Trait, this.Entity.Id, this.typeName, i.Key, i.Value.Get());
            }
        }

        private void Invoke(AbstractMeta proxy, CallType callType, SyncType syncType, ulong authorityOwnerId, string subTarget, params object[] payload)
        {
            var world = ((EntityCreator)this.Entity.Creator).World;
            switch (world.Mode)
            {
            case WorldMode.Standalone:
                proxy.Call(payload);
                break;
            case WorldMode.Server:
                switch (proxy.Domain)
                {
                case ApplyDomain.Client:
                    world.Do(authorityOwnerId, syncType, SyncTarget.Trait, this.Entity.Id, this.typeName, subTarget, payload);
                    break;
                case ApplyDomain.Server:
                    proxy.Call(payload);
                    break;
                case ApplyDomain.NetMultiple:
                    proxy.Call(payload);
                    if (this.Entity.Replicated)
                    {
                        world.Do(authorityOwnerId, syncType, SyncTarget.Trait, this.Entity.Id, this.typeName, subTarget, payload);
                    }
                    break;
                }
                break;
            case WorldMode.Client:
                switch (proxy.Domain)
                {
                case ApplyDomain.Client:
                    if (CallType.Local == callType || CanInvoke(authorityOwnerId, proxy.Condition))
                    {
                        proxy.Call(payload);
                    }
                    break;
                case ApplyDomain.Server:
                    world.Do(authorityOwnerId, syncType, SyncTarget.Trait, this.Entity.Id, this.typeName, subTarget, payload);
                    break;
                case ApplyDomain.NetMultiple:
                    if (CallType.Local == callType)
                    {
                        proxy.Call(payload);
                        if (this.Entity.Replicated)
                        {
                            world.Do(authorityOwnerId, syncType, SyncTarget.Trait, this.Entity.Id, this.typeName, subTarget, payload);
                        }
                    }
                    else if (CanInvoke(authorityOwnerId, proxy.Condition))
                    {
                        proxy.Call(payload);
                    }
                    break;
                }
                break;
            }
        }

        private bool CanInvoke(ulong authorityOwnerId, ApplyCondition condition)
        {
            return ((ApplyCondition.All == condition) ||
                    (ApplyCondition.OwnerOnly == condition && authorityOwnerId == ((EntityCreator)this.Entity.Creator).Id) ||
                    (ApplyCondition.SkipOwner == condition && authorityOwnerId != ((EntityCreator)this.Entity.Creator).Id));
        }

        private Type GetMethodType(Type methodProxy)
        {
            var staticFormat = methodProxy.GetField("StaticFormat", BindingFlags.Static | BindingFlags.NonPublic);
            return (Type)staticFormat.GetValue(null);
        }
    }
}
