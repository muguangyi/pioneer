/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Net.Sockets;

namespace Pioneer
{
    sealed class NetPeer : IPeer
    {
        public NetPeer(Socket socket, ISerializer serializer)
        {
            this.Socket = socket ?? throw new ArgumentNullException("Socket can't be null!");
            this.Serializer = serializer ?? throw new ArgumentNullException("Serializer can't be null!");
        }

        public void Send(object obj)
        {
            throw new global::System.NotImplementedException();
        }

        public void Close()
        {
            if (this.Socket != null)
            {
                this.Socket.Shutdown(SocketShutdown.Both);
                this.Socket.Close();
                this.Socket = null;
            }
        }

        internal Socket Socket { get; private set; } = null;
        internal ISerializer Serializer { get; } = null;
    }
}
