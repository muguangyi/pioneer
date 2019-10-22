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
    abstract class FrameObject : IDisposable
    {
        public virtual void Dispose()
        { }

        public virtual void BeginFrame(float deltaTime)
        { }

        public virtual void EndFrame(float deltaTime)
        { }
    }
}
