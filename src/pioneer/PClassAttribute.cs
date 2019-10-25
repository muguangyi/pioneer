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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PClassAttribute : Attribute
    {
        public PClassAttribute(ApplyDomain domain = ApplyDomain.NetMultiple)
        {
            this.Domain = domain;
        }

        /// <summary>
        /// Gets a value to indicate the apply domain.
        /// </summary>
        public ApplyDomain Domain { get; private set; }
    }
}
