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
    /// Actor interface.
    /// </summary>
    public interface IActor : ITraitContainer, IControlContainer, IDisposable
    {
        /// <summary>
        /// Gets a value to indicate the actor creator.
        /// </summary>
        ICreator Creator { get; }

        /// <summary>
        /// Gets a value to indicate the world instance.
        /// </summary>
        IWorld World { get; }
    }
}
