/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer
{
    public abstract class AbstractArgs : AbstractMeta
    {
        public AbstractArgs(ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        { }

        public override void Call(params object[] args)
        { }
    }

    public sealed class PArgs : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke();
        }

        private Method method = null;

        internal static Type StaticFormat = typeof(Method);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public sealed class PArgs<T1> : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0]);
        }

        private Method<T1> method = null;

        internal static Type StaticFormat = typeof(Method<T1>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public sealed class PArgs<T1, T2> : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1]);
        }

        private Method<T1, T2> method = null;

        internal static Type StaticFormat = typeof(Method<T1, T2>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public sealed class PArgs<T1, T2, T3> : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2, T3>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1], (T3)args[2]);
        }

        private Method<T1, T2, T3> method = null;

        internal static Type StaticFormat = typeof(Method<T1, T2, T3>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public sealed class PArgs<T1, T2, T3, T4> : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2, T3, T4>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
        }

        private Method<T1, T2, T3, T4> method = null;

        internal static Type StaticFormat = typeof(Method<T1, T2, T3, T4>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public sealed class PArgs<T1, T2, T3, T4, T5> : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2, T3, T4, T5>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]);
        }

        private Method<T1, T2, T3, T4, T5> method = null;

        internal static Type StaticFormat = typeof(Method<T1, T2, T3, T4, T5>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public sealed class PArgs<T1, T2, T3, T4, T5, T6> : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2, T3, T4, T5, T6>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5]);
        }

        private Method<T1, T2, T3, T4, T5, T6> method = null;

        internal static Type StaticFormat = typeof(Method<T1, T2, T3, T4, T5, T6>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public sealed class PArgs<T1, T2, T3, T4, T5, T6, T7> : AbstractArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2, T3, T4, T5, T6, T7>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6]);
        }

        private Method<T1, T2, T3, T4, T5, T6, T7> method = null;

        internal static Type StaticFormat = typeof(Method<T1, T2, T3, T4, T5, T6, T7>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    public sealed class PArgs<T1, T2, T3, T4, T5, T6, T7, T8> : AbstractArgs
    {
        internal static Type StaticFormat = typeof(Method<T1, T2, T3, T4, T5, T6, T7, T8>);

        private Method<T1, T2, T3, T4, T5, T6, T7, T8> method = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2, T3, T4, T5, T6, T7, T8>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    public sealed class PArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> : AbstractArgs
    {
        private Method<T1, T2, T3, T4, T5, T6, T7, T8, T9> method = null;

        internal static Type StaticFormat = typeof(Method<T1, T2, T3, T4, T5, T6, T7, T8, T9>);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        /// <param name="condition"></param>
        public PArgs(Delegate method, ApplyDomain domain, ApplyCondition condition) : base(domain, condition)
        {
            this.method = (Method<T1, T2, T3, T4, T5, T6, T7, T8, T9>)method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Call(params object[] args)
        {
            this.method?.Invoke((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8]);
        }
    }
}
