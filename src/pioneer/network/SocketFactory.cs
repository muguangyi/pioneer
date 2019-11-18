/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Pioneer.Network
{
    /// <summary>
    /// Internal socket factory to create target socket object based on network service provider.
    /// </summary>
    static class SocketFactory
    {
        private static readonly Dictionary<string, Func<string, ISerializer, ISocket>> makers = new Dictionary<string, Func<string, ISerializer, ISocket>>();

        static SocketFactory()
        {
            Extend("tcp", (url, serializer) => { return new TcpSocket(url, serializer); });
        }

        /// <summary>
        /// Create a new socket with network service provider and inject packet serializer.
        /// </summary>
        /// <param name="nsp">Network service provider.</param>
        /// <param name="serializer">Network packet serializer.</param>
        /// <returns></returns>
        public static ISocket Create(string nsp, ISerializer serializer)
        {
            var parts = Parse(nsp);
            if (makers.TryGetValue(parts.Item1, out Func<string, ISerializer, ISocket> maker))
            {
                return maker(parts.Item2, serializer);
            }

            return null;
        }

        /// <summary>
        /// Extend socket maker with protocol type string.
        /// </summary>
        /// <param name="protocol">Protocol type string.</param>
        /// <param name="maker">The socket maker.</param>
        public static void Extend(string protocol, Func<string, ISerializer, ISocket> maker)
        {
            if (makers.ContainsKey(protocol))
            {
                makers[protocol] = maker;
            }
            else
            {
                makers.Add(protocol, maker);
            }
        }

        private static (string, string) Parse(string nsp)
        {
            const string Separater = "://";
            var index = nsp.IndexOf(Separater);
            if (index < 0)
            {
                return (string.Empty, string.Empty);
            }

            return (nsp.Substring(0, index), nsp.Substring(index + Separater.Length));
        }
    }
}
