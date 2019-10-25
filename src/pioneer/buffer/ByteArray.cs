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

namespace Pioneer.Buffer
{
    sealed class ByteArray : IByteArray
    {
        private readonly ByteBlock block = null;
        private ByteArray prev = null;

        public ByteArray(ByteBlock block, int offset, int total)
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

        public byte[] Buffer
        {
            get
            {
                return this.block.Buffer;
            }
        }

        public int Total { get; private set; } = 0;

        public int Offset { get; } = 0;

        public int Size { get; private set; } = 0;

        public int Position { get; private set; } = 0;

        public ByteArray Next { get; private set; } = null;

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
            this.Position = 0;

            var size = ByteStorage.GetProperSize(this.Size);
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

        internal void SplitByteArray(int size)
        {
            var splitNode = new ByteArray(this.block, this.Offset + size, this.Total - size);
            this.Total = size;

            if (null != this.Next)
            {
                this.Next.prev = splitNode;
                splitNode.Next = this.Next;
            }

            this.Next = splitNode;
            splitNode.prev = this;
        }

        private void Reset()
        {
            this.Size = 0;
            this.Position = 0;
        }
    }
}