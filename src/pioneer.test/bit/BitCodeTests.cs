﻿/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pioneer.Bit;
using System;

namespace Pioneer.Test.Bit
{
    [TestClass]
    public class BitCodeTests
    {
        [ExpectedException(typeof(IndexOutOfRangeException))]
        [DataRow(BitCode.MAXSIZE)]
        [DataRow(BitCode.MAXSIZE + 1)]
        [DataRow(uint.MaxValue)]
        [DataTestMethod]
        public void TestBitCodeBoundary(uint boundary)
        {
            new BitCode(boundary);
        }

        [DataRow(1u, 2u)]
        [DataRow(1u, 3u)]
        [DataRow(1u, 1000u)]
        [DataRow(0u, BitCode.MAXSIZE - 1)]
        [DataTestMethod]
        public void TestCompositeAddContains(uint index1, uint index2)
        {
            var a = new BitCode(index1);
            var b = new BitCode(index2);

            var c = BitCode.CompositeCode.Create();
            c.Add(a);
            c.Add(b);

            Assert.IsTrue(c.Contains(a));
            Assert.IsTrue(c.Contains(b));
        }

        [DataRow(1u, 2u)]
        [DataRow(1u, 3u)]
        [DataRow(1u, 1000u)]
        [DataRow(0u, BitCode.MAXSIZE - 1)]
        [DataTestMethod]
        public void TestCompositeSubtractContains(uint index1, uint index2)
        {
            var a = new BitCode(index1);
            var b = new BitCode(index2);

            var c = BitCode.CompositeCode.Create();
            c.Add(a);
            c.Add(b);

            Assert.IsTrue(c.Contains(a));
            Assert.IsTrue(c.Contains(b));

            c.Subtract(b);

            Assert.IsTrue(c.Contains(a));
            Assert.IsFalse(c.Contains(b));

            c.Subtract(a);

            Assert.IsFalse(c.Contains(a));
            Assert.IsFalse(c.Contains(b));
        }
    }
}