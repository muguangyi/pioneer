﻿/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer
{
    enum SyncType : byte
    {
        Authorize,
        Loaded,
        Entered,
        Exited,
        Create,
        Destroy,
        SetProp,
        CallFunc,
        HeartBeat,
    }

    enum SyncTarget : byte
    {
        World,
        Player,
        Entity,
        Trait,
        Tag,
    }

    struct SyncAction
    {
        public ulong OwnerId;
        public SyncType Type;
        public SyncTarget Target;
        public ulong EntityId;
        public string ClsName;
        public string SubTarget;
        public object[] Payload;
    }
}