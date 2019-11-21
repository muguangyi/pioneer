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
    abstract class FrameObject : IDisposable
    {
        [ExcludeFromCodeCoverage]
        public virtual void Dispose()
        { }

        public virtual void BeginFrame(float deltaTime)
        { }

        public virtual void EndFrame(float deltaTime)
        { }
    }
}
