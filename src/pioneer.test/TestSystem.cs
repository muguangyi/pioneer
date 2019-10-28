/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Pioneer.Test
{
    [TestClass]
    public class TestSystem
    {
        [TestInitialize]
        public void Setup()
        {
            Env.Start();
        }

        [TestCleanup]
        public void Teardown()
        {
            Env.Stop();
        }

        [TestMethod]
        public void TestVerifyJobEntityGroup()
        {
            var world = Env.GetWorld();
            var a = world.CreateEntity();
            a.AddTrait<ATrait>();
            a.AddTrait<BTrait>();
            var b = world.CreateEntity();
            b.AddTrait<ATrait>();
            b.AddTrait<BTrait>();
            var system = world.AddSystem<ABJobSystem>();

            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(system.Filter.Target.Count(), 2);

            a.RemoveTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(system.Filter.Target.Count(), 1);

            a.AddTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(system.Filter.Target.Count(), 2);

            a.RemoveTrait<BTrait>();
            b.RemoveTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(system.Filter.Target.Count(), 0);
        }

        [TestMethod]
        public void TestVerifyReactiveEntityGroup()
        {
            var world = Env.GetWorld();
            var a = world.CreateEntity();
            var aa = a.AddTrait<ATrait>();
            var ab = a.AddTrait<BTrait>();
            var b = world.CreateEntity();
            var ba = b.AddTrait<ATrait>();
            var bb = b.AddTrait<BTrait>();
            var system = world.AddSystem<ABReactSystem>();

            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 2);

            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 0);

            aa.ChangeValue();
            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 1);

            aa.ChangeValue();
            ab.ChangeValue();
            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 1);

            aa.ChangeValue();
            bb.ChangeValue();
            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 2);

            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 0);

            Env.UpdateWorld(1);
            aa.ChangeValue();
            Assert.AreEqual(system.ReactCount, 0);
            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 1);
            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 0);
        }

        [TestMethod]
        public void TestVerifyDelayReactiveEntity()
        {
            var world = Env.GetWorld();
            var a = world.CreateEntity();
            var aa = a.AddTrait<ATrait>();
            var ab = a.AddTrait<BTrait>();
            var b = world.CreateEntity();
            var ba = b.AddTrait<ATrait>();
            var bb = b.AddTrait<BTrait>();
            var system = world.AddSystem<ABReactSystem>();
            var system2 = world.AddSystem<ABModifySystem>();

            int lastModifyCount = 0;

            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 2);
            lastModifyCount = system2.ModifyCount;

            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, lastModifyCount);
            lastModifyCount = system2.ModifyCount;

            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, lastModifyCount);

            a.RemoveTrait<ATrait>();
            b.RemoveTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.AreEqual(system.ReactCount, 0);
        }

        [TestMethod]
        public void TestDeleteEntity()
        {
            var world = Env.GetWorld();
            var a = world.CreateEntity();
            var aa = a.AddTrait<ATrait>();
            var ab = a.AddTrait<BTrait>();
            var b = world.CreateEntity();
            var ba = b.AddTrait<ATrait>();
            var bb = b.AddTrait<BTrait>();
            var system = world.AddSystem<ABJobDeleteSystem>();

            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(system.Filter.Target.Count(), 2);

            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(system.Filter.Target.Count(), 1);

            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(system.Filter.Target.Count(), 0);
        }

        [TestMethod]
        public void TestVerifyMatcherSystem()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var l = world.AddSystem<MatcherTestSystem>();

            Assert.AreEqual(l.Filter.Target.Count(), 0);

            e.AddTrait<ATrait>();
            e.AddTrait<BTrait>();
            e.AddTrait<CTrait>();
            e.AddTrait<DTrait>();
            e.AddTag("E");
            e.AddTag("F");
            Assert.AreEqual(l.Filter.Target.Count(), 0);

            e.RemoveTrait<BTrait>();
            Assert.AreEqual(l.Filter.Target.Count(), 0);

            e.RemoveTrait<DTrait>();
            Assert.AreEqual(l.Filter.Target.Count(), 0);

            e.RemoveTag("F");
            Assert.AreEqual(l.Filter.Target.Count(), 1);

            e.AddTag("F");
            Assert.AreEqual(l.Filter.Target.Count(), 0);
        }

        [TestMethod]
        public void TestModifyFilterSystemTest()
        {
            var world = Env.GetWorld();

            var e1 = world.CreateEntity();
            e1.AddTrait<ATrait>();

            var e2 = world.CreateEntity();
            e2.AddTrait<BTrait>();

            var s = world.AddSystem<ModifyFilterSystem>();

            Assert.AreEqual(s.Filter.Target.Count(), 1);

            Env.UpdateWorld(1);
            Assert.AreEqual(s.Filter.Target.Count(), 0);

            var e3 = world.CreateEntity();
            e3.AddTrait<ATrait>();

            Assert.AreEqual(s.Filter.Target.Count(), 1);

            Env.UpdateWorld(1);
            Assert.AreEqual(s.Filter.Target.Count(), 0);
        }

        [TestMethod]
        public void TestVerifySystemDispose()
        {
            var world = Env.GetWorld();

            world.AddSystem<NoneSystem>();
            world.AddSystem<ABJobSystem>();
            world.AddSystem<ABReactSystem>();
            world.AddSystem<ABJobDeleteSystem>();
            world.AddSystem<MatcherTestSystem>();
            world.AddSystem<ModifyFilterSystem>();

            Assert.AreEqual(BaseSystem.SystemCount, 6);

            Env.Stop();

            Assert.AreEqual(BaseSystem.SystemCount, 0);
        }

        [TestMethod]
        public void TestVerifySharedJobFilters()
        {
            var world = Env.GetWorld();
            var s1 = world.AddSystem<SameJobMatcherSystem1>();
            var s2 = world.AddSystem<SameJobMatcherSystem2>();

            Assert.AreEqual(s1.Matcher, s2.Matcher);
            Assert.AreEqual(s1.Filter, s2.Filter);
        }

        [TestMethod]
        public void TestVerifyDifferntReactFilters()
        {
            var world = Env.GetWorld();
            var s1 = world.AddSystem<SameReactMatcherSystem1>();
            var s2 = world.AddSystem<SameReactMatcherSystem2>();

            Assert.AreEqual(s1.Matcher, s2.Matcher);
            Assert.AreNotEqual(s1.Filter, s2.Filter);
        }
    }
}
