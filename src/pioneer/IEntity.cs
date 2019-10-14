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
    /// Entity interface.
    /// </summary>
    public interface IEntity : ITraitContainer, IControlContainer, IDisposable
    {
        /// <summary>
        /// Gets a value to indicate the entity creator.
        /// </summary>
        IEntityCreator Creator { get; }
    }
}
