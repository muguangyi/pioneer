﻿/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Network;

namespace Pioneer.Framework
{
    sealed class Player : Creator
    {
        public Player(ulong id, World world, IPeer peer) : base(id, world)
        {
            this.Peer = peer;
        }

        public override bool HasAuthority => base.HasAuthority || null != this.Peer;

        public IPeer Peer { get; private set; }
    }
}
