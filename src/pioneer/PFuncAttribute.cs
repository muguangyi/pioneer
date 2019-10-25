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
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PFuncAttribute : Attribute
    {
        public PFuncAttribute(Type format, ApplyDomain domain = ApplyDomain.NetMultiple, ApplyCondition condition = ApplyCondition.All)
        {
            this.Format = format;
            this.Domain = domain;
            this.Condition = condition;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type Format { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplyDomain Domain { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplyCondition Condition { get; private set; }
    }
}
