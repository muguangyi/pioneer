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

namespace Pioneer.network
{
    static class SocketFactory
    {
        private static readonly Dictionary<string, Func<string, ISerializer, ISocket>> makers = new Dictionary<string, Func<string, ISerializer, ISocket>>();

        static SocketFactory()
        {
            Extend("tcp", (url, serializer) => { return new TcpSocket(url, serializer); });
        }

        public static ISocket Create(string nsp, ISerializer serializer)
        {
            var parts = Parse(nsp);
            if (makers.TryGetValue(parts.Item1, out Func<string, ISerializer, ISocket> maker))
            {
                return maker(parts.Item2, serializer);
            }

            return null;
        }

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
