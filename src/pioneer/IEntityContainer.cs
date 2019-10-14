/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Pioneer
{
    /// <summary>
    /// Entity container interface.
    /// </summary>
    public interface IEntityContainer : IEntityCreator, IEntitiesBinder
    {
        /// <summary>
        /// Event when a player entity enters the container.
        /// </summary>
        event Action<IEntity> OnPlayerEntered;

        /// <summary>
        /// Event when a player entity exits the container.
        /// </summary>
        event Action<IEntity> OnPlayerExited;

        /// <summary>
        /// Gets a value to indicate the entity collection in the container.
        /// </summary>
        IEnumerable<IEntity> Entities { get; }

        /// <summary>
        /// Get entity by entity ID.
        /// </summary>
        /// <param name="entityId">Entity ID.</param>
        /// <returns>Entity instance if exists, otherwise return null.</returns>
        IEntity GetEntityById(ulong entityId);
    }
}
