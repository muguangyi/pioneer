/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Pioneer.Buffer
{
    sealed class BufStorage : IDisposable
    {
        private const int MIN_BUFFER_SIZE = 64 * 1024;  // 64k
        private const int MIN_BYTEARRAY_SIZE = 32;      // 32b

        private readonly List<BufBlock> blocks = new List<BufBlock>();

        public static int GetProperSize(int size)
        {
            if (size <= MIN_BYTEARRAY_SIZE)
            {
                return MIN_BYTEARRAY_SIZE;
            }

            int total = MIN_BYTEARRAY_SIZE;
            while (total < size)
            {
                total <<= 1;
            }

            return total;
        }

        public BufStorage()
        {
            this.blocks.Add(new BufBlock(this, MIN_BUFFER_SIZE));
        }

        public void Dispose()
        {
            for (var i = 0; i < this.blocks.Count; ++i)
            {
                this.blocks[i].Dispose();
            }
        }

        public IBufArray Alloc(int size)
        {
            BufArray bytes = null;
            for (int i = 0; i < this.blocks.Count; ++i)
            {
                bytes = this.blocks[i].Capture(size);
                if (bytes != null)
                {
                    return bytes;
                }
            }

            var block = new BufBlock(this, Math.Max(GetProperSize(size), MIN_BUFFER_SIZE));
            this.blocks.Add(block);

            return block.Capture(size);
        }
    }
}
