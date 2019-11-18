/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Net;

namespace Pioneer.Network
{
    /// <summary>
    /// An isolate interface to seperate real low level socket.
    /// Only export the methods that peer needed.
    /// </summary>
    interface INetProxy
    {
        /// <summary>
        /// Listen the end point and will trigger the callback when accepts a new connection.
        /// </summary>
        /// <param name="endPoint">Local end point.</param>
        /// <param name="payload">Payload object.</param>
        /// <param name="callback">Callback handler.</param>
        void ListenAsync(EndPoint endPoint, object payload, Action<INetProxy, object> callback);

        /// <summary>
        /// Dial to the end point and will trigger the callback when connects successfully.
        /// </summary>
        /// <param name="endPoint">Remote end point.</param>
        /// <param name="payload">Payload object.</param>
        /// <param name="callback">Callback handler.</param>
        void DialAsync(EndPoint endPoint, object payload, Action<object> callback);

        /// <summary>
        ///  Sends the specified number of bytes of data to a connected System.Net.Sockets.Socket,
        ///  starting at the specified offset.
        /// </summary>
        /// <param name="buffer">An array of type System.Byte that contains the data to be sent.</param>
        /// <param name="offset">The position in the data buffer at which to begin sending data.</param>
        /// <param name="size">The number of bytes to send.</param>
        /// <returns>The number of bytes sent to the proxy.</returns>
        int Send(byte[] buffer, int offset, int size);

        /// <summary>
        /// Receives the specified number of bytes from a bound System.Net.Sockets.Socket
        /// into the specified offset position of the receive buffer.
        /// </summary>
        /// <param name="buffer">An array of type System.Byte that is the storage location for received data.</param>
        /// <param name="offset">The location in buffer to store the received data.</param>
        /// <param name="size">The number of bytes to receive.</param>
        /// <returns>The number of bytes received.</returns>
        int Receive(byte[] buffer, int offset, int size);

        /// <summary>
        /// Close the proxy.
        /// </summary>
        void Close();
    }
}
