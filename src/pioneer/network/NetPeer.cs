/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Net.Sockets;

namespace Pioneer
{
    sealed class NetPeer : IPeer
    {
        private readonly Socket socket = null;

        public NetPeer(Socket socket)
        {
            this.socket = socket;
        }

        public void Send(object obj)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
