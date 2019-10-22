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
    abstract class ReusableObject : IReusableObject, IDisposable
    {
        public virtual void Dispose()
        { }

        /// <inheritdoc/>
        public virtual void Active()
        { }

        /// <inheritdoc/>
        public virtual void Deactive()
        { }
    }
}
