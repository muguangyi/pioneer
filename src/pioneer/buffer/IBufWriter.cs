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
    interface IBufWriter
    {
        void WriteByte(byte data);

        void WriteBool(bool data);

        void WriteUInt16(UInt16 data);

        void WriteUInt32(UInt32 data);

        void WriteUInt64(UInt64 data);

        void WriteInt16(Int16 data);

        void WriteInt32(Int32 data);

        void WriteInt64(Int64 data);

        void WriteInt(int data);

        void WriteFloat(float data);

        void WriteString(string data);

        void WriteBytes(byte[] data, int offset, int size);
    }
}
