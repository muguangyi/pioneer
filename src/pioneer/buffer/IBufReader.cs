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

        UInt16 ReadUInt16();

        UInt32 ReadUInt32();

        UInt64 ReadUInt64();

        Int16 ReadInt16();

        Int32 ReadInt32();

        Int64 ReadInt64();

        int ReadInt();

        float ReadFloat();

        double ReadDouble();

        string ReadString();

        byte[] ReadBytes(int length = -1);
    }
}
