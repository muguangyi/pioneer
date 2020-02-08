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

namespace Pioneer.Test.Pioneer
{
    [TestClass]
    public class ActorTests
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
        public void TestActorIdNotZero()
        {
            var world = Env.GetWorld();
            var actor = world.CreateActor();
            Assert.AreNotEqual(0, actor.Id);
        }

        [TestMethod]
        public void TestMultipleEnityIdNotEqual()
        {
            var world = Env.GetWorld();
            var a1 = world.CreateActor();
            var a2 = world.CreateActor();
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(a1.Id, a2.Id);
        }

        [TestMethod]
        public void TestDestroyActor()
        {
            var world = Env.GetWorld();
            var a = world.CreateActor();
            var id = a.Id;
            Assert.IsNotNull(a);

            a.Dispose();
            Env.UpdateWorld(1);
            Assert.IsNull(world.GetActorById(id));
        }

        [TestMethod]
        public void TestAddTrait()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            var a = e.AddTrait<ATrait>();
            var b = e.AddTrait<BTrait>();
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
        }

        [TestMethod]
        public void TestAddTrait2()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            var a = e.AddTrait(typeof(ATrait));
            var b = e.AddTrait(typeof(BTrait));
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
        }

        [TestMethod]
        public void TestAddTrait3()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            var a = e.AddTrait("Pioneer.Test.Pioneer.Support.ATrait");
            var b = e.AddTrait("Pioneer.Test.Pioneer.Support.BTrait");
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
        }

        [TestMethod]
        public void TestCantAddSameTraits()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            var ac = e.AddTrait<ATrait>();
            Assert.AreEqual(ac, e.AddTrait(typeof(ATrait)));
        }

        [TestMethod]
        public void TestCheckTraitExist()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();

            Assert.IsFalse(e.HasTrait<ATrait>());

            var ac = e.AddTrait<ATrait>();
            Assert.IsTrue(e.HasTrait<ATrait>());

            e.RemoveTrait<ATrait>();
            Assert.IsFalse(e.HasTrait<ATrait>());
        }

        [TestMethod]
        public void TestVerifyGetDataTuple()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            e.AddTrait<ATrait>();
            e.AddTrait<BTrait>();
            var logic = e.AddControl<ABJobControl>();

            Env.UpdateWorld(1);
            Assert.IsTrue(logic.Filter.Matched);

            e.RemoveTrait<BTrait>();
            Env.UpdateWorld(1);
            Assert.IsFalse(logic.Filter.Matched);
        }

        [TestMethod]
        public void TestVerifyTraitChanged()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            var a = e.AddTrait<ATrait>();
            var b = e.AddTrait<BTrait>();
            var logic = e.AddControl<ABReactControl>();

            Assert.IsTrue(logic.Filter.Matched);
            Env.UpdateWorld(1);
            Assert.IsFalse(logic.Filter.Matched);

            a.ChangeValue();
            Assert.IsTrue(logic.Filter.Matched);
            Env.UpdateWorld(1);

            b.ChangeValue();
            Assert.IsTrue(logic.Filter.Matched);
            Env.UpdateWorld(1);

            a.ChangeValue();
            b.ChangeValue();
            Assert.IsTrue(logic.Filter.Matched);
            Env.UpdateWorld(1);
            Assert.IsFalse(logic.Filter.Matched);
        }

        [TestMethod]
        public void TestVerifyMatcher()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            var a = e.AddTrait<ATrait>();
            var b = e.AddTrait<BTrait>();
            e.AddTag("tag");
            var l1 = e.AddControl<ABJobControl>();
            var l2 = e.AddControl<AWithoutBJobControl>();
            var l3 = e.AddControl<CTagJobControl>();

            Assert.IsTrue(l1.Filter.Matched);
            Assert.IsFalse(l2.Filter.Matched);
            Assert.IsFalse(l3.Filter.Matched);

            e.RemoveTrait<BTrait>();
            Assert.IsFalse(l1.Filter.Matched);
            Assert.IsTrue(l2.Filter.Matched);
            Assert.IsFalse(l3.Filter.Matched);

            e.AddTag("C");
            Assert.IsFalse(l1.Filter.Matched);
            Assert.IsTrue(l2.Filter.Matched);
            Assert.IsTrue(l3.Filter.Matched);

            e.RemoveTag("C");
            Assert.IsFalse(l1.Filter.Matched);
            Assert.IsTrue(l2.Filter.Matched);
            Assert.IsFalse(l3.Filter.Matched);
        }

        [TestMethod]
        public void TestVerifyTraitDispose()
        {
            var world = Env.GetWorld();
            var e = world.CreateActor();
            e.AddTrait<ATrait>();
            e.AddTrait<BTrait>();
            e.AddTrait<CTrait>();
            e.AddTrait<DTrait>();

            Assert.AreEqual(BaseTrait.TraitCount, 4);

            e.Dispose();
            Env.UpdateWorld(1);

            Assert.AreEqual(BaseTrait.TraitCount, 0);
        }

        [TestMethod]
        public void TestVerifyContrlDispose()
        {
            var world = Env.GetWorld();

            var e = world.CreateActor();
            e.AddTrait<ATrait>();
            e.AddTrait<BTrait>();

            e.AddControl<ABJobControl>();
            e.AddControl<AWithoutBJobControl>();
            e.AddControl<ABReactControl>();
            e.AddControl<CTagJobControl>();
            Assert.AreEqual(BaseControl.ControlCount, 4);
        }

        [TestMethod]
        public void TestVerifyReusableEntities()
        {
            var world = Env.GetWorld();

            var e0 = world.CreateActor();
            e0.AddTrait<ATrait>();

            Env.UpdateWorld(1);

            var id0 = e0.Id;
            e0.Dispose();

            var e1 = world.CreateActor();
            Assert.AreNotEqual(id0, e1.Id);

            Env.UpdateWorld(1);

            var e2 = world.CreateActor();
            Assert.AreEqual(e0, e2);    // reuse the same actor, but all content is clean
            Assert.AreNotEqual(id0, e2.Id);
            Assert.AreNotEqual(e1.Id, e2.Id);
            Assert.IsNull(e2.GetTrait<ATrait>());
        }
    }
}
