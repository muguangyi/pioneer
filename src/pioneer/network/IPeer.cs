/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer.Network
{
    /// <summary>
    /// Network peer.
    /// </summary>
    interface IPeer
    {
        /// <summary>
        /// Gets a value to indicate the peer id.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// Send an object.
        /// </summary>
        /// <param name="obj">Object.</param>
        void Send(object obj);
    }
}
