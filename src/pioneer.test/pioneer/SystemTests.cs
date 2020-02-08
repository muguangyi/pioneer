/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pioneer.Test.Pioneer.Support;
using System.Linq;

namespace Pioneer.Test.Pioneer
{
    [TestClass]
    public class SystemTests
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
        public void TestVerifyJobActorGroup()
        {
            var world = Env.GetWorld();
            var a = world.CreateActor();
            a.AddTrait<ATrait>();
            a.AddTrait<BTrait>();
            var b = world.CreateActor();
            b.AddTrait<ATrait>();
            b.AddTrait<BTrait>();
            var system = world.AddSystem<ABJobSystem>();

            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(2, system.Filter.Actors.Count());

            a.RemoveTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(1, system.Filter.Actors.Count());

            a.AddTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(2, system.Filter.Actors.Count());

            a.RemoveTrait<BTrait>();
            b.RemoveTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.AreEqual(0, system.Filter.Actors.Count());
        }

        [TestMethod]
        public void TestVerifyReactiveActorGroup()
        {
            var world = Env.GetWorld();
            var a = world.CreateActor();
            var aa = a.AddTrait<ATrait>();
            var ab = a.AddTrait<BTrait>();
            var b = world.CreateActor();
            var ba = b.AddTrait<ATrait>();
            var bb = b.AddTrait<BTrait>();
            var system = world.AddSystem<ABReactSystem>();

            Env.UpdateWorld(1);
            Assert.AreEqual(2, system.ReactCount);

            Env.UpdateWorld(1);
            Assert.AreEqual(0, system.ReactCount);

            aa.ChangeValue();
            Env.UpdateWorld(1);
            Assert.AreEqual(1, system.ReactCount);

            aa.ChangeValue();
            ab.ChangeValue();
            Env.UpdateWorld(1);
            Assert.AreEqual(1, system.ReactCount);

            aa.ChangeValue();
            bb.ChangeValue();
            Env.UpdateWorld(1);
            Assert.AreEqual(2, system.ReactCount);

            Env.UpdateWorld(1);
            Assert.AreEqual(0, system.ReactCount);

            Env.UpdateWorld(1);
            aa.ChangeValue();
            Assert.AreEqual(0, system.ReactCount);
            Env.UpdateWorld(1);
            Assert.AreEqual(1, system.ReactCount);
            Env.UpdateWorld(1);
            Assert.AreEqual(0, system.ReactCount);
        }

        [TestMethod]
        public void TestVerifyDelayReactiveActor()
        {
            var world = Env.GetWorld();
            var a = world.CreateActor();
            var aa = a.AddTrait<ATrait>();
            var ab = a.AddTrait<BTrait>();
            var b = world.CreateActor();
            var ba = b.AddTrait<ATrait>();
            var bb = b.AddTrait<BTrait>();
            var system = world.AddSystem<ABReactSystem>();
            var system2 = world.AddSystem<ABModifySystem>();

            int lastModifyCount = 0;

            Env.UpdateWorld(1);
            Assert.AreEqual(2, system.ReactCount);
            lastModifyCount = system2.ModifyCount;

            Env.UpdateWorld(1);
            Assert.AreEqual(lastModifyCount, system.ReactCount);
            lastModifyCount = system2.ModifyCount;

            Env.UpdateWorld(1);
            Assert.AreEqual(lastModifyCount, system.ReactCount);

            a.RemoveTrait<ATrait>();
            b.RemoveTrait<ATrait>();
            Env.UpdateWorld(1);
            Assert.AreEqual(0, system.ReactCount);
        }

        [TestMethod]
        public void TestDeleteActor()
        {
            var world = Env.GetWorld();
            var a = world.CreateActor();
            var aa = a.AddTrait<ATrait>();
            var ab = a.AddTrait<BTrait>();
            var b = world.CreateActor();
            var ba = b.AddTrait<ATrait>();
            var bb = b.AddTrait<BTrait>();
            var system = world.AddSystem<ABJobDeleteSystem>();

            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(2, system.Filter.Actors.Count());

            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(1, system.Filter.Actors.Count());

            Env.UpdateWorld(1);
            Assert.IsNotNull(system.Filter);
            Assert.AreEqual(0, system.Filter.Actors.Count());
        }

        [TestMethod]
        public void TestVerifyMatcherSystem()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            var l = world.AddSystem<MatcherTestSystem>();

            Assert.AreEqual(0, l.Filter.Actors.Count());

            e.AddTrait<ATrait>();
            e.AddTrait<BTrait>();
            e.AddTrait<CTrait>();
            e.AddTrait<DTrait>();
            e.AddTag("E");
            e.AddTag("F");
            Assert.AreEqual(0, l.Filter.Actors.Count());

            e.RemoveTrait<BTrait>();
            Assert.AreEqual(0, l.Filter.Actors.Count());

            e.RemoveTrait<DTrait>();
            Assert.AreEqual(0, l.Filter.Actors.Count());

            e.RemoveTag("F");
            Assert.AreEqual(1, l.Filter.Actors.Count());

            e.AddTag("F");
            Assert.AreEqual(0, l.Filter.Actors.Count());
        }

        [TestMethod]
        public void TestModifyFilterSystemTest()
        {
            var world = Env.GetWorld();

            var e1 = world.CreateActor();
            e1.AddTrait<ATrait>();

            var e2 = world.CreateActor();
            e2.AddTrait<BTrait>();

            var s = world.AddSystem<ModifyFilterSystem>();

            Assert.AreEqual(1, s.Filter.Actors.Count());

            Env.UpdateWorld(1);
            Assert.AreEqual(0, s.Filter.Actors.Count());

            var e3 = world.CreateActor();
            e3.AddTrait<ATrait>();

            Assert.AreEqual(1, s.Filter.Actors.Count());

            Env.UpdateWorld(1);
            Assert.AreEqual(0, s.Filter.Actors.Count());
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

            Assert.AreEqual(6, BaseSystem.SystemCount);

            Env.Stop();

            Assert.AreEqual(0, BaseSystem.SystemCount);
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
