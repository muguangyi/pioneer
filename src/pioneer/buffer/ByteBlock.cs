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
    sealed class ByteBlock : IDisposable
    {
        private readonly ByteArray header = null;

        public ByteBlock(ByteStorage storage, int size)
        {
            this.Storage = storage;
            this.Buffer = new byte[size];
            this.header = new ByteArray(this, 0, size);
        }

        public void Dispose()
        {
            this.Buffer = null;
        }

        public ByteStorage Storage { get; } = null;

        public byte[] Buffer { get; private set; } = null;

        public ByteArray Capture(int size)
        {
            size = ByteStorage.GetProperSize(size);

            lock (this)
            {
                var node = this.header;
                while (null != node)
                {
                    if (0 == node.RefCount && node.Total >= size)
                    {
                        if (node.Total >= (size << 1))
                        {
                            node.SplitByteArray(size);
                        }

                        node.Retain();
                        return node;
                    }

                    node = node.Next;
                }
            }

            return null;
        }
    }
}