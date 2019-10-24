/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Net;
using System.Net.Sockets;

namespace Pioneer
{
    class TcpSocket : NetSocket
    {
        public TcpSocket(string url, ISerializer serializer)
            : base(
                new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                MakeEndPoint(url),
                serializer)
        { }

        private static EndPoint MakeEndPoint(string url)
        {
            return null;
        }
    }
}
