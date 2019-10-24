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
    interface ISocket
    {
        event Action<IPeer> OnConnected;
        event Action<IPeer, Exception> OnClosed;
        event Action<IPeer, object> OnPacket;

        bool Connected { get; }
        void Listen();
        void Dial();
        void Close();
    }
}
