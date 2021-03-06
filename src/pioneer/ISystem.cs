﻿/*
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
    /// System interface.
    /// </summary>
    public interface ISystem : IDisposable
    {
        /// <summary>
        /// Initialize entrance for the system component.
        /// </summary>
        /// <param name="world">The world container.</param>
        void OnInit(IWorld world);

        /// <summary>
        /// Delta update entrance for the system component.
        /// </summary>
        /// <param name="world">The world container.</param>
        /// <param name="deltaTime">Delta time from the last frame.</param>
        void OnUpdate(IWorld world, float deltaTime);
    }
}
