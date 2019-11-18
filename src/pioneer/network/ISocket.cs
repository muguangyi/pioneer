/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer.Network
{
    /// <summary>
    /// Network socket.
    /// </summary>
    interface ISocket
    {
        /// <summary>
        /// Event when a peer connected.
        /// </summary>
        event Action<IPeer> OnConnected;

        /// <summary>
        /// Event when a peer closed.
        /// </summary>
        event Action<IPeer, Exception> OnClosed;

        /// <summary>
        /// Event when a message received.
        /// </summary>
        event Action<IPeer, object> OnMessage;

        /// <summary>
        /// Get a value to indicate whether the socket is connected.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Listen an incomming connection.
        /// </summary>
        void Listen();

        /// <summary>
        /// Dial to target server.
        /// </summary>
        void Dial();

        /// <summary>
        /// Close the network socket.
        /// </summary>
        void Close();
    }
}
