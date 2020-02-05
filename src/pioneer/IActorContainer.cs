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
    /// Actor container interface.
    /// </summary>
    public interface IActorContainer : ICreator, IActorsBinder
    {
        /// <summary>
        /// Event when a player actor enters the container.
        /// </summary>
        event Action<IActor> OnPlayerEntered;

        /// <summary>
        /// Event when a player actor exits the container.
        /// </summary>
        event Action<IActor> OnPlayerExited;

        /// <summary>
        /// Gets a value to indicate the actor collection in the container.
        /// </summary>
        IEnumerable<IActor> Actors { get; }

        /// <summary>
        /// Get actor by actor's ID.
        /// </summary>
        /// <param name="actorId">Actor ID.</param>
        /// <returns>Actor instance if exists, otherwise return null.</returns>
        IActor GetActorById(ulong actorId);
    }
}
