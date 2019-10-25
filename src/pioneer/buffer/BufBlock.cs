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
    sealed class BufBlock : IDisposable
    {
        private readonly BufSlice header = null;

        public BufBlock(BufStorage storage, int size)
        {
            this.Storage = storage;
            this.Buffer = new byte[size];
            this.header = new BufSlice(this, 0, size);
        }

        public void Dispose()
        {
            this.Buffer = null;
        }

        public BufStorage Storage { get; } = null;

        public byte[] Buffer { get; private set; } = null;

        public BufSlice Capture(int size)
        {
            size = BufStorage.GetProperSize(size);

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