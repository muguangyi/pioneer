/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Diagnostics.CodeAnalysis;

namespace Pioneer.Base
{
    abstract class InstantObject : IDisposable
    {
        private static uint instanceIndex = 0;

        public InstantObject()
        {
            this.InstanceId = (++instanceIndex);
        }

        public uint InstanceId { get; }

        [ExcludeFromCodeCoverage]
        public virtual void Dispose()
        { }

        public static void Reset()
        {
            instanceIndex = 0;
        }
    }
}
