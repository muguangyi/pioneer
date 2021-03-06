/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.IO;
using System.Text;

namespace Pioneer.Buffer
{
    sealed class BufSlice : IBufSlice
    {
        public static bool BigEndian { get; set; } = true;

        private readonly BufBlock block = null;
        private BufSlice prev = null;
        private bool readOnly = false;

        public BufSlice(BufBlock block, int offset, int total)
        {
            this.block = block;
            this.Offset = offset;
            this.Total = total;
            Reset();
        }

        public void Dispose()
        {
            Reset();

            if (null != this.Next && 0 == this.Next.RefCount)
            {
                this.Total += this.Next.Total;
                this.Next = this.Next.Next;
                if (null != this.Next)
                {
                    this.Next.prev = this;
                }
            }

            if (null != this.prev && 0 == this.prev.RefCount)
            {
                this.prev.Total += this.Total;
                this.prev.Next = this.Next;
                if (null != this.Next)
                {
                    this.Next.prev = this.prev;
                }
            }
        }

        public int RefCount { get; private set; } = 0;

        public byte[] Buffer => this.block.Buffer;

        public int Total { get; private set; } = 0;

        public int Offset { get; } = 0;

        public int Size { get; private set; } = 0;

        public int Position { get; private set; } = 0;

        public BufSlice Next { get; private set; } = null;

        public void Retain()
        {
            ++this.RefCount;
        }

        public void Release()
        {
            lock (this.block)
            {
                if (0 == (--this.RefCount))
                {
                    Dispose();
                }
            }
        }

        public void Submit()
        {
            this.readOnly = true;
            this.Position = 0;

            var size = BufStorage.GetProperSize(this.Size);
            if (this.Total >= (size << 1))
            {
                SplitByteArray(size);
            }
        }

        public void Seek(int offset = 0, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            switch (seekOrigin)
            {
            case SeekOrigin.Begin:
                this.Position = offset;
                break;
            case SeekOrigin.Current:
                this.Position += offset;
                break;
            case SeekOrigin.End:
                this.Position = this.Size + offset;
                break;
            }
        }

        public void SetSize(int size)
        {
            if (size > this.Total)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.Size = size;
        }

        public byte ReadByte()
        {
            VertifyInRange(1);

            byte value = this.block.Buffer[this.Offset + this.Position];
            this.Position += 1;

            return value;
        }

        public bool ReadBool()
        {
            byte value = ReadByte();
            return (0 != value);
        }

        public ushort ReadUInt16()
        {
            VertifyInRange(2);

            ushort value = 0;
            if (BigEndian)
            {
                for (var i = 0; i < 2; i++)
                {
                    value = (ushort)(value << 8);
                    value |= (ushort)this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            else
            {
                for (var i = 1; i >= 0; i--)
                {
                    value = (ushort)(value << 8);
                    value |= (ushort)this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            this.Position += 2;

            return value;
        }

        public uint ReadUInt32()
        {
            VertifyInRange(4);

            uint value = 0;
            if (BigEndian)
            {
                for (var i = 0; i < 4; i++)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            else
            {
                for (var i = 3; i >= 0; i--)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            this.Position += 4;

            return value;
        }

        public ulong ReadUInt64()
        {
            VertifyInRange(8);

            ulong value = 0;
            if (BigEndian)
            {
                for (var i = 0; i < 8; i++)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            else
            {
                for (var i = 7; i >= 0; i--)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            this.Position += 8;

            return value;
        }

        public short ReadInt16()
        {
            VertifyInRange(2);

            short value = 0;
            if (BigEndian)
            {
                for (var i = 0; i < 2; i++)
                {
                    value = (short)(value << 8);
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            else
            {
                for (var i = 1; i >= 0; i--)
                {
                    value = (short)(value << 8);
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            this.Position += 2;

            return value;
        }

        public int ReadInt32()
        {
            VertifyInRange(4);

            int value = 0;
            if (BigEndian)
            {
                for (var i = 0; i < 4; i++)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            else
            {
                for (var i = 3; i >= 0; i--)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            this.Position += 4;

            return value;
        }

        public long ReadInt64()
        {
            VertifyInRange(8);

            long value = 0;
            if (BigEndian)
            {
                for (var i = 0; i < 8; i++)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            else
            {
                for (var i = 7; i >= 0; i--)
                {
                    value = value << 8;
                    value |= this.block.Buffer[this.Offset + this.Position + i];
                }
            }
            this.Position += 8;

            return value;
        }

        public int ReadInt()
        {
            return ReadInt32();
        }

        public float ReadFloat()
        {
            VertifyInRange(4);
            float value;
            if (BigEndian)
            {
                value = BitConverter.ToSingle(this.block.Buffer, this.Offset + this.Position);
            }
            else
            {
                var tmp = new byte[4]
                {
                    this.block.Buffer[this.Offset + this.Position + 3],
                    this.block.Buffer[this.Offset + this.Position + 2],
                    this.block.Buffer[this.Offset + this.Position + 1],
                    this.block.Buffer[this.Offset + this.Position],
                };
                value = BitConverter.ToSingle(tmp, 0);
            }
            this.Position += 4;

            return value;
        }

        public double ReadDouble()
        {
            VertifyInRange(8);
            double value;
            if (BigEndian)
            {
                value = BitConverter.ToDouble(this.block.Buffer, this.Offset + this.Position);
            }
            else
            {
                var tmp = new byte[8]
                {
                    this.block.Buffer[this.Offset + this.Position + 7],
                    this.block.Buffer[this.Offset + this.Position + 6],
                    this.block.Buffer[this.Offset + this.Position + 5],
                    this.block.Buffer[this.Offset + this.Position + 4],
                    this.block.Buffer[this.Offset + this.Position + 3],
                    this.block.Buffer[this.Offset + this.Position + 2],
                    this.block.Buffer[this.Offset + this.Position + 1],
                    this.block.Buffer[this.Offset + this.Position],
                };
                value = BitConverter.ToDouble(tmp, 0);
            }
            this.Position += 8;

            return value;
        }

        public string ReadString()
        {
            int length = ReadUInt16();
            VertifyInRange(length);

            string value = Encoding.UTF8.GetString(this.block.Buffer, this.Offset + this.Position, length);
            this.Position += length;

            return value;
        }

        public byte[] ReadBytes(int length = -1)
        {
            if (-1 == length)
            {
                length = this.Size - this.Position;
            }
            VertifyInRange(length);

            var bytes = new byte[length];
            Array.Copy(this.block.Buffer, this.Offset + this.Position, bytes, 0, length);
            this.Position += length;

            return bytes;
        }

        public void WriteByte(byte data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(1);

            this.block.Buffer[this.Offset + this.Position] = data;
            this.Position += 1;

            UpdateSize(this.Position);
        }

        public void WriteBool(bool data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(1);

            WriteByte(data ? (byte)1 : (byte)0);
        }

        public void WriteUInt16(ushort data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(2);

            if (BigEndian)
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 24 >> 24);
            }
            else
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 24 >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 16 >> 24);
            }

            this.Position += 2;

            UpdateSize(this.Position);
        }

        public void WriteUInt32(uint data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(4);

            if (BigEndian)
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 8 >> 24);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data << 24 >> 24);
            }
            else
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 24 >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 8 >> 24);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data >> 24);
            }

            this.Position += 4;

            UpdateSize(this.Position);
        }

        public void WriteUInt64(ulong data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(8);

            if (BigEndian)
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data >> 56);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 8 >> 56);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 16 >> 56);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data << 24 >> 56);
                this.block.Buffer[this.Offset + this.Position + 4] = (byte)(data << 32 >> 56);
                this.block.Buffer[this.Offset + this.Position + 5] = (byte)(data << 40 >> 56);
                this.block.Buffer[this.Offset + this.Position + 6] = (byte)(data << 48 >> 56);
                this.block.Buffer[this.Offset + this.Position + 7] = (byte)(data << 56 >> 56);
            }
            else
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 56 >> 56);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 48 >> 56);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 40 >> 56);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data << 32 >> 56);
                this.block.Buffer[this.Offset + this.Position + 4] = (byte)(data << 24 >> 56);
                this.block.Buffer[this.Offset + this.Position + 5] = (byte)(data << 16 >> 56);
                this.block.Buffer[this.Offset + this.Position + 6] = (byte)(data << 8 >> 56);
                this.block.Buffer[this.Offset + this.Position + 7] = (byte)(data >> 56);
            }
            this.Position += 8;

            UpdateSize(this.Position);
        }

        public void WriteInt16(short data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(2);

            if (BigEndian)
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 24 >> 24);
            }
            else
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 24 >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 16 >> 24);
            }
            this.Position += 2;

            UpdateSize(this.Position);
        }

        public void WriteInt32(int data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(4);

            if (BigEndian)
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 8 >> 24);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data << 24 >> 24);
            }
            else
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 24 >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 8 >> 24);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data >> 24);
            }
            this.Position += 4;

            UpdateSize(this.Position);
        }

        public void WriteInt64(long data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(8);

            if (BigEndian)
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data >> 56);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 8 >> 56);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 16 >> 56);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data << 24 >> 56);
                this.block.Buffer[this.Offset + this.Position + 4] = (byte)(data << 32 >> 56);
                this.block.Buffer[this.Offset + this.Position + 5] = (byte)(data << 40 >> 56);
                this.block.Buffer[this.Offset + this.Position + 6] = (byte)(data << 48 >> 56);
                this.block.Buffer[this.Offset + this.Position + 7] = (byte)(data << 56 >> 56);
            }
            else
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 56 >> 56);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 48 >> 56);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 40 >> 56);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data << 32 >> 56);
                this.block.Buffer[this.Offset + this.Position + 4] = (byte)(data << 24 >> 56);
                this.block.Buffer[this.Offset + this.Position + 5] = (byte)(data << 16 >> 56);
                this.block.Buffer[this.Offset + this.Position + 6] = (byte)(data << 8 >> 56);
                this.block.Buffer[this.Offset + this.Position + 7] = (byte)(data >> 56);
            }
            this.Position += 8;

            UpdateSize(this.Position);
        }

        public void WriteInt(int data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(4);

            if (BigEndian)
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 8 >> 24);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data << 24 >> 24);
            }
            else
            {
                this.block.Buffer[this.Offset + this.Position] = (byte)(data << 24 >> 24);
                this.block.Buffer[this.Offset + this.Position + 1] = (byte)(data << 16 >> 24);
                this.block.Buffer[this.Offset + this.Position + 2] = (byte)(data << 8 >> 24);
                this.block.Buffer[this.Offset + this.Position + 3] = (byte)(data >> 24);
            }
            this.Position += 4;

            UpdateSize(this.Position);
        }

        public void WriteFloat(float data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(4);

            byte[] bytes = BitConverter.GetBytes(data);
            if (!BigEndian)
            {
                Array.Reverse(bytes);
            }
            this.block.Buffer[this.Offset + this.Position] = bytes[0];
            this.block.Buffer[this.Offset + this.Position + 1] = bytes[1];
            this.block.Buffer[this.Offset + this.Position + 2] = bytes[2];
            this.block.Buffer[this.Offset + this.Position + 3] = bytes[3];
            this.Position += 4;

            UpdateSize(this.Position);
        }

        public void WriteDouble(double data)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(8);

            byte[] bytes = BitConverter.GetBytes(data);
            if (!BigEndian)
            {
                Array.Reverse(bytes);
            }
            this.block.Buffer[this.Offset + this.Position] = bytes[0];
            this.block.Buffer[this.Offset + this.Position + 1] = bytes[1];
            this.block.Buffer[this.Offset + this.Position + 2] = bytes[2];
            this.block.Buffer[this.Offset + this.Position + 3] = bytes[3];
            this.block.Buffer[this.Offset + this.Position + 4] = bytes[4];
            this.block.Buffer[this.Offset + this.Position + 5] = bytes[5];
            this.block.Buffer[this.Offset + this.Position + 6] = bytes[6];
            this.block.Buffer[this.Offset + this.Position + 7] = bytes[7];
            this.Position += 8;

            UpdateSize(this.Position);
        }

        public void WriteString(string data)
        {
            if (this.readOnly)
            {
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            VerifyCapacity(bytes.Length + 2);

            WriteUInt16((ushort)bytes.Length);
            for (int i = 0; i < bytes.Length; ++i)
            {
                this.block.Buffer[this.Offset + this.Position + i] = bytes[i];
            }
            this.Position += (bytes.Length + 2);

            UpdateSize(this.Position);
        }

        public void WriteBytes(byte[] data, int offset, int size)
        {
            if (this.readOnly)
            {
                return;
            }

            VerifyCapacity(size);

            Array.Copy(data, offset, this.block.Buffer, this.Offset + this.Position, size);
            this.Position += size;

            UpdateSize(this.Position);
        }

        internal void SplitByteArray(int size)
        {
            var splitNode = new BufSlice(this.block, this.Offset + size, this.Total - size);
            this.Total = size;

            if (null != this.Next)
            {
                this.Next.prev = splitNode;
                splitNode.Next = this.Next;
            }

            this.Next = splitNode;
            splitNode.prev = this;
        }

        private void UpdateSize(int size)
        {
            if (this.Size < size)
            {
                this.Size = size;
            }
        }

        private void VerifyCapacity(int needSize)
        {
            if (this.Total - this.Position < needSize)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void VertifyInRange(int needSize)
        {
            if (this.Size - this.Position < needSize)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void Reset()
        {
            this.Size = 0;
            this.Position = 0;
            this.readOnly = false;
        }
    }
}