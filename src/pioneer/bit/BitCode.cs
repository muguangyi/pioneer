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
using System.Diagnostics.CodeAnalysis;

namespace Pioneer.Bit
{
    struct BitCode
    {
        public const uint MAXSIZE = MAXOFFSET * BITCOUNT;

        private const byte MAXOFFSET = 16;
        private const byte BITCOUNT = 64;

        private ulong[] bits;
        private byte offset;

        public BitCode(uint index)
        {
            if (index >= MAXSIZE)
            {
                throw new IndexOutOfRangeException($"Index range can't excceed {MAXSIZE - 1}!");
            }

            this.Index = index;
            this.bits = new ulong[MAXOFFSET];

            this.offset = (byte)(index / BITCOUNT);
            this.bits[this.offset] = (ulong)1 << (byte)(index % BITCOUNT);
        }

        public uint Index { get; }

        public struct CompositeCode
        {
            [ExcludeFromCodeCoverage]
            public override bool Equals(object obj)
            {
                if (obj is CompositeCode)
                {
                    var c = (CompositeCode)obj;
                    if (c.Offset != this.Offset)
                    {
                        return false;
                    }

                    for (var i = 0; i <= this.Offset; ++i)
                    {
                        if (c.bits[i] != this.bits[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }

            [ExcludeFromCodeCoverage]
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public void Add(BitCode code)
            {
                if (this.indices.Add(code.Index))
                {
                    this.Offset = Math.Max(code.offset, this.Offset);
                    for (var i = 0; i <= code.offset && i < MAXOFFSET; ++i)
                    {
                        this.bits[i] |= code.bits[i];
                    }
                }
            }

            public void Subtract(BitCode code)
            {
                if (this.indices.Remove(code.Index))
                {
                    for (var i = 0; i <= code.offset && i < MAXOFFSET; ++i)
                    {
                        this.bits[i] &= ~code.bits[i];
                    }
                }
            }

            public bool Contains(BitCode code)
            {
                for (var i = 0; i <= code.offset && i < MAXOFFSET; ++i)
                {
                    if ((this.bits[i] & code.bits[i]) != code.bits[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool Contains(CompositeCode code)
            {
                for (var i = 0; i <= code.Offset && i < MAXOFFSET; ++i)
                {
                    if ((this.bits[i] & code.bits[i]) != code.bits[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool Intersect(CompositeCode code)
            {
                for (var i = 0; i <= code.Offset && i < MAXOFFSET; ++i)
                {
                    if ((this.bits[i] & code.bits[i]) != 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            public byte Offset { get; private set; }

            public IEnumerable<uint> Indices
            {
                get
                {
                    return this.indices;
                }
            }

            private ulong[] bits;
            private HashSet<uint> indices;

            public static CompositeCode Create()
            {
                return new CompositeCode
                {
                    bits = new ulong[MAXOFFSET],
                    Offset = 0,
                    indices = new HashSet<uint>(),
                };
            }

            public static bool operator ==(CompositeCode c0, CompositeCode c1)
            {
                if (c0.Offset != c1.Offset)
                {
                    return false;
                }

                for (var i = 0; i <= c0.Offset; ++i)
                {
                    if (c0.bits[i] != c1.bits[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public static bool operator !=(CompositeCode c0, CompositeCode c1)
            {
                return !(c0 == c1);
            }
        }
    }
}
