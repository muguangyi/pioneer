/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Buffer;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Pioneer
{
    sealed class NetPeer : IPeer
    {
        private const int BUFFER_SIZE = 256 * 1024; // 256k

        private readonly BufStorage storage = new BufStorage();
        private readonly Thread sendThread = null;
        private readonly Thread recvThread = null;
        private readonly Queue<object> sendQueue = new Queue<object>();
        private readonly AutoResetEvent sendWatcher = new AutoResetEvent(false);
        private IBufArray sendBuffer = null;
        private IBufArray recvBuffer = null;
        private bool closed = false;

        public NetPeer(Socket socket, ISerializer serializer)
        {
            this.Socket = socket ?? throw new ArgumentNullException("Socket can't be null!");
            this.Serializer = serializer ?? throw new ArgumentNullException("Serializer can't be null!");
            this.sendBuffer = this.storage.Alloc(BUFFER_SIZE);
            this.recvBuffer = this.storage.Alloc(BUFFER_SIZE);
            this.sendThread = new Thread(new ThreadStart(DoSendAsync)); this.sendThread.Start();
            this.recvThread = new Thread(new ThreadStart(DoRecvAsync)); this.recvThread.Start();
        }

        public void Send(object obj)
        {
            lock (this.sendQueue)
            {
                this.sendQueue.Enqueue(obj);
            }
            this.sendWatcher.Set();
        }

        public void Close()
        {
            this.closed = true;

            if (this.Socket != null)
            {
                this.Socket.Shutdown(SocketShutdown.Both);
                this.Socket.Close();
                this.Socket = null;
            }

            this.storage.Dispose();
            this.sendBuffer = null;
            this.recvBuffer = null;
        }

        private void DoSendAsync()
        {
            while (!this.closed)
            {
                if (this.sendQueue.Count == 0)
                {
                    this.sendWatcher.WaitOne();
                }

                while (this.sendQueue.Count > 0)
                {
                    try
                    {
                        this.sendBuffer.Seek();
                        this.sendBuffer.SetSize(0);
                        lock (this.sendQueue)
                        {
                            this.Serializer.Marshal(this.sendQueue.Dequeue(), this.sendBuffer);
                        }
                        this.sendBuffer.Seek();

                        int size = -1;
                        while ((size = this.Socket.Send(this.sendBuffer.Buffer, this.sendBuffer.Offset + this.sendBuffer.Position, this.sendBuffer.Size - this.sendBuffer.Position, SocketFlags.None)) < this.sendBuffer.Size)
                        {
                            this.sendBuffer.Seek(size);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private void DoRecvAsync()
        {
            while (!this.closed)
            {
                try
                {
                    var size = this.Socket.Receive(this.recvBuffer.Buffer, this.recvBuffer.Offset + this.recvBuffer.Position, BUFFER_SIZE - this.recvBuffer.Position, SocketFlags.None);
                    if (size > 0)
                    {
                        this.recvBuffer.SetSize(this.recvBuffer.Position + size);
                        this.recvBuffer.Seek();
                        this.Serializer.Unmarshal(this.recvBuffer, out object obj);

                        var unreadSize = this.recvBuffer.Size - this.recvBuffer.Position;
                        if (unreadSize > 0)
                        {
                            Array.Copy(this.recvBuffer.Buffer, this.recvBuffer.Offset + this.recvBuffer.Position, this.recvBuffer.Buffer, this.recvBuffer.Offset, unreadSize);
                        }
                        this.recvBuffer.Seek(unreadSize);
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        internal Socket Socket { get; private set; } = null;
        internal ISerializer Serializer { get; } = null;
    }
}
