/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pioneer.Base;

namespace Pioneer.Test.Base
{
    [TestClass]
    public class InstantObjectTests
    {
        private class AObject : InstantObject
        { }

        private class BObject : InstantObject
        { }

        [TestInitialize]
        public void Initialize()
        {
            InstantObject.Reset();
        }

        [TestMethod]
        public void TestInstanceId()
        {
            var a = new AObject();
            var b = new BObject();
            Assert.AreEqual<uint>(1, a.InstanceId);
            Assert.AreEqual<uint>(2, b.InstanceId);
        }
    }
}
