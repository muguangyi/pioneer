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
    /// Base container interface.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Gets a value to indicate the container ID.
        /// </summary>
        ulong Id { get; }
    }
}
