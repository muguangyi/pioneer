/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;

namespace Pioneer
{
    /// <summary>
    /// Actors' filter interface.
    /// </summary>
    public interface IActorsFilter
    {
        /// <summary>
        /// Gets a value to indicate the actor count.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a value to indicate the actor collection.
        /// </summary>
        IEnumerable<IActor> Target { get; }

        /// <summary>
        /// Gets a value to indicate the first actor.
        /// </summary>
        IActor First { get; }
    }
}
