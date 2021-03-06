﻿/*
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
    /// Group filter interface.
    /// </summary>
    public interface IGroupFilter : IFilter
    {
        /// <summary>
        /// Gets a value to indicate the actor collection.
        /// </summary>
        IEnumerable<IActor> Actors { get; }
    }
}
