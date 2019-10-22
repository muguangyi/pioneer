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
    abstract class InstantObject : IDisposable
    {
        private static uint instanceIndex = 0;

        public InstantObject()
        {
            this.InstanceId = (++instanceIndex);
        }

        public virtual void Dispose()
        { }

        public uint InstanceId { get; }
    }
}
