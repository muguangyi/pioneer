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
        private static ulong PeerIndex = 0;

        private NetPeer peer = null;
        private EndPoint endPoint = null;

        public NetSocket(Socket socket, EndPoint endPoint, ISerializer serializer)
        {
            this.peer = new NetPeer(PeerIndex++, socket, serializer);
            this.peer.OnClosed += OnPeerClosed;
            this.peer.OnMessage += OnPeerMessage;
            this.endPoint = endPoint ?? throw new ArgumentNullException("EndPoint can't be null!");
        }

        public bool Connected => throw new NotImplementedException();

        public event Action<IPeer> OnConnected;
        public event Action<IPeer, Exception> OnClosed;
        public event Action<IPeer, object> OnMessage;

        public void Listen()
        {
            this.peer.Socket.Bind(this.endPoint);
            this.peer.Socket.Listen(5);
            this.peer.Socket.BeginAccept(OnEndAccept, this.peer);
        }

        public void Dial()
        {
            this.peer.Socket.BeginConnect(this.endPoint, OnEndConnect, this.peer);
        }

        public void Close()
        {
            CloseInternal(this.peer);
        }

        private void OnEndAccept(IAsyncResult result)
        {
            var peer = (NetPeer)result.AsyncState;
            try
            {
                var socket = peer.Socket.EndAccept(result);
                var p = new NetPeer(PeerIndex++, socket, peer.Serializer);
                p.OnClosed += OnPeerClosed;
                p.OnMessage += OnPeerMessage;
                this.OnConnected?.Invoke(p);
            }
            catch
            { }
        }

        private void OnEndConnect(IAsyncResult result)
        {
            var peer = (NetPeer)result.AsyncState;
            try
            {
                peer.Socket.EndConnect(result);
                this.OnConnected?.Invoke(peer);
            }
            catch (Exception ex)
            {
                CloseInternal(peer, ex);
            }
        }

        private void CloseInternal(NetPeer peer, Exception ex = null)
        {
            if (peer != null)
            {
                peer.Close(ex);
            }
        }

        private void OnPeerClosed(IPeer peer, Exception ex)
        {
            this.OnClosed?.Invoke(peer, ex);
        }

        private void OnPeerMessage(IPeer peer, object obj)
        {
            this.OnMessage?.Invoke(peer, obj);
        }
    }
}
