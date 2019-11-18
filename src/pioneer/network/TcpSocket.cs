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

namespace Pioneer.Network
{
    class TcpSocket : NetSocket
    {
        public TcpSocket(string url, ISerializer serializer)
            : base(MakeProxy(), MakeEndPoint(url), serializer)
        { }

        private static INetProxy MakeProxy()
        {
            return new SocketProxy(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        }

        private static EndPoint MakeEndPoint(string url)
        {
            var address = url.Split(':');
            return new IPEndPoint(IPAddress.Parse(address[0]), int.Parse(address[1]));
        }
    }
}
