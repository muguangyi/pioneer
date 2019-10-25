/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Sync;
using System;
using System.Reflection;

namespace Pioneer
{
    /// <summary>
    /// Base abstract meta class.
    /// </summary>
    public abstract class AbstractMeta : IDisposable
    {
        public AbstractMeta(ApplyDomain domain, ApplyCondition condition)
        {
            this.Domain = domain;
            this.Condition = condition;
        }

        public ApplyDomain Domain { get; private set; }

        public ApplyCondition Condition { get; private set; }

        public virtual void Dispose()
        { }

        public virtual void Set(object value)
        { }

        public virtual object Get()
        {
            return null;
        }

        public virtual void Call(params object[] args)
        {
            Set(args[0]);
        }
    }

    /// <summary>
    /// Meta class with target Type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PMeta<T> : AbstractMeta
    {
        public PMeta(object defaultValue, PropertyInfo property, object target, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.defaultValue = (null != defaultValue ? (T)defaultValue : default(T));

            var getMethod = property.GetGetMethod();
            if (null != getMethod)
            {
                this.getFunc = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), target, getMethod);
            }

            var setMethod = property.GetSetMethod();
            if (null != setMethod)
            {
                this.setFunc = (Method<T>)Delegate.CreateDelegate(typeof(Method<T>), target, setMethod);
                Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            Reset();
        }

        public override void Set(object value)
        {
            this.setFunc?.Invoke((T)value);
        }

        public override object Get()
        {
            if (null != this.getFunc)
            {
                return this.getFunc();
            }

            return default(T);
        }

        private void Reset()
        {
            this.setFunc?.Invoke(this.defaultValue);   // Reset value
        }

        private T defaultValue = default(T);
        private Method<T> setFunc = null;
        private Func<T> getFunc = null;
    }
}
