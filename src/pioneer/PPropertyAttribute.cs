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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PPropertyAttribute : Attribute
    {
        public PPropertyAttribute(Type format, object defaultValue = null, ApplyDomain domain = ApplyDomain.NetMultiple, ApplyCondition condition = ApplyCondition.All)
        {
            this.Format = format;
            this.DefaultValue = defaultValue;
            this.Domain = domain;
            this.Condition = condition;
        }

        /// <summary>
        /// Gets a value to indicate the property format.
        /// </summary>
        public Type Format { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object DefaultValue { get; private set; }

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
