/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pioneer.Bit;

namespace Pioneer.Test.Bit
{
    [TestClass]
    public class BitCodeCenterTests
    {
        private class AClass
        { }

        [TestMethod]
        public void TestBitCodeEqual()
        {
            var center = new BitCodeCenter();
            var c1 = center.GetBitCodeByType(typeof(AClass));
            var c2 = center.GetBitCodeByString("Pioneer.Test.Bit.BitCodeCenterTests+AClass");
            Assert.AreEqual(c1, c2);
        }

        [TestMethod]
        public void TestBitCodeNotEqual()
        {
            var center = new BitCodeCenter();
            var c1 = center.GetBitCodeByType(typeof(AClass));
            var c2 = center.GetBitCodeByString("AClass");
            Assert.AreNotEqual(c1, c2);
        }
    }
}
