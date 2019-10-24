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

namespace Pioneer
{
    abstract class NetSocket : ISocket
    {
        private Socket socket = null;
        private EndPoint endPoint = null;
        private ISerializer serializer = null;

        public NetSocket(Socket socket, EndPoint endPoint, ISerializer serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException("Socket can't be null!");
            this.endPoint = endPoint ?? throw new ArgumentNullException("EndPoint can't be null!");
            this.serializer = serializer ?? throw new ArgumentNullException("Serializer can't be null!");
        }

        public bool Connected => throw new NotImplementedException();

        public event Action<IPeer> OnConnected;
        public event Action<IPeer, Exception> OnClosed;
        public event Action<IPeer, object> OnPacket;

        public void Listen()
        {
            this.socket.Bind(this.endPoint);
            this.socket.Listen(5);
            this.socket.BeginAccept(OnEndAccept, this.socket);
        }

        public void Dial()
        {
            this.socket.BeginConnect(this.endPoint, OnEndConnect, this.socket);
        }

        public void Close()
        {
            CloseInternal();
        }

        private void OnEndAccept(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            try
            {
                var s = socket.EndAccept(result);
                this.OnConnected?.Invoke(new NetPeer(s));
            }
            catch (Exception ex)
            {
                CloseInternal(ex);
            }
        }

        private void OnEndConnect(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            try
            {
                socket.EndConnect(result);
                this.OnConnected?.Invoke(new NetPeer(socket));
            }
            catch (Exception ex)
            {
                CloseInternal(ex);
            }
        }

        private void CloseInternal(Exception ex = null)
        {
            if (this.socket != null)
            {
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                this.socket = null;
            }

            this.OnClosed?.Invoke(null, ex);
        }
    }
}
