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
    interface IBufWriter : IBufStream
    {
        void WriteByte(byte data);

        void WriteBool(bool data);

        void WriteUInt16(ushort data);

        void WriteUInt32(uint data);

        void WriteUInt64(ulong data);

        void WriteInt16(short data);

        void WriteInt32(int data);

        void WriteInt64(long data);

        void WriteInt(int data);

        void WriteFloat(float data);

        void WriteDouble(double data);

        void WriteString(string data);

        void WriteBytes(byte[] data, int offset, int size);
    }
}
