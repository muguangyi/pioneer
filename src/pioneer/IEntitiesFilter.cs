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
    /// Entities filter interface.
    /// </summary>
    public interface IEntitiesFilter
    {
        /// <summary>
        /// Gets a value to indicate the entity count.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a value to indicate the entity collection.
        /// </summary>
        IEnumerable<IEntity> Target { get; }

        /// <summary>
        /// Gets a value to indicate the first entity.
        /// </summary>
        IEntity First { get; }
    }
}
