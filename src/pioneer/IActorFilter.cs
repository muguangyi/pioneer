/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer
{
    /// <summary>
    /// Actor filter interface.
    /// </summary>
    public interface IActorFilter
    {
        /// <summary>
        /// Gets a value to indicate the owner actor.
        /// </summary>
        IActor Target { get; }
    }
}
