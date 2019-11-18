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
using System.Net.Sockets;

namespace Pioneer.Network
{
    /// <summary>
    /// Proxy to wrap System.Net.Socket.
    /// </summary>
    class SocketProxy : INetProxy
    {
        private readonly Socket socket;

        public SocketProxy(Socket socket)
        {
            this.socket = socket;
        }

        /// <inheritdoc />
        public void ListenAsync(EndPoint endPoint, object payload, Action<INetProxy, object> callback)
        {
            this.socket.Bind(endPoint);
            this.socket.Listen(5);
            this.socket.BeginAccept(result =>
            {
                callback?.Invoke(new SocketProxy(this.socket.EndAccept(result)), payload);
            }, payload);
        }

        /// <inheritdoc />
        public void DialAsync(EndPoint endPoint, object payload, Action<object> callback)
        {
            this.socket.BeginConnect(endPoint, result =>
            {
                this.socket.EndConnect(result);
                callback?.Invoke(payload);
            }, payload);
        }

        /// <inheritdoc />
        public int Send(byte[] buffer, int offset, int size)
        {
            return this.socket.Send(buffer, offset, size, SocketFlags.None);
        }

        /// <inheritdoc />
        public int Receive(byte[] buffer, int offset, int size)
        {
            return this.socket.Receive(buffer, offset, size, SocketFlags.None);
        }

        /// <inheritdoc />
        public void Close()
        {
            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();
        }
    }
}
