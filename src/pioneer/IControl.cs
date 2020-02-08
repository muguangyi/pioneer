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
    /// Control component interface.
    /// </summary>
    public interface IControl : IDisposable
    {
        /// <summary>
        /// Initialize entrance for the control component.
        /// </summary>
        /// <param name="actor">The owner trait container.</param>
        void OnInit(IActor actor);

        /// <summary>
        /// Delta update entrance for the control component.
        /// </summary>
        /// <param name="actor">The owner trait container.</param>
        /// <param name="deltaTime">Delta time from the last frame.</param>
        void OnUpdate(IActor actor, float deltaTime);
    }
}
