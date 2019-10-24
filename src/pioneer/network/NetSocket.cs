/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer
{
    abstract class NetSocket : ISocket
    {
        public bool Connected => throw new NotImplementedException();

        public event Action<IPeer> OnConnected;
        public event Action<IPeer> OnClosed;
        public event Action<IPeer, object> OnPacket;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dial()
        {
            throw new NotImplementedException();
        }

        public void Listen()
        {
            throw new NotImplementedException();
        }
    }
}
