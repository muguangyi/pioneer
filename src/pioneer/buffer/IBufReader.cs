/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer.Buffer
{
    interface IBufReader : IBufStream
    {
        byte ReadByte();

        bool ReadBool();

        ushort ReadUInt16();

        uint ReadUInt32();

        ulong ReadUInt64();

        short ReadInt16();

        int ReadInt32();

        long ReadInt64();

        int ReadInt();

        float ReadFloat();

        double ReadDouble();

        string ReadString();

        byte[] ReadBytes(int length = -1);
    }
}
