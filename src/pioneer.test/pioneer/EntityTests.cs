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
    public class EntityTests
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
        public void TestEntityIdNotZero()
        {
            var world = Env.GetWorld();
            var entity = world.CreateEntity();
            Assert.AreNotEqual(entity.Id, 0);
        }

        [TestMethod]
        public void TestMultipleEnityIdNotEqual()
        {
            var world = Env.GetWorld();
            var e1 = world.CreateEntity();
            var e2 = world.CreateEntity();
            Assert.AreNotEqual(e1, e2);
            Assert.AreNotEqual(e1.Id, e2.Id);
        }

        [TestMethod]
        public void TestDestroyEntity()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var id = e.Id;
            Assert.IsNotNull(e);

            e.Dispose();
            Env.UpdateWorld(1);
            Assert.IsNull(world.GetEntityById(id));
        }

        [TestMethod]
        public void TestAddTrait()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var a = e.AddTrait<ATrait>();
            var b = e.AddTrait<BTrait>();
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
        }

        [TestMethod]
        public void TestAddTrait2()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var a = e.AddTrait(typeof(ATrait));
            var b = e.AddTrait(typeof(BTrait));
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
        }

        [TestMethod]
        public void TestAddTrait3()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var a = e.AddTrait("Pioneer.Test.Pioneer.Support.ATrait");
            var b = e.AddTrait("Pioneer.Test.Pioneer.Support.BTrait");
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
        }

        [TestMethod]
        public void TestCantAddSameTraits()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var ac = e.AddTrait<ATrait>();
            Assert.AreEqual(ac, e.AddTrait(typeof(ATrait)));
        }

        [TestMethod]
        public void TestCheckTraitExist()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();

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
            var e = world.CreateEntity();
            e.AddTrait<ATrait>();
            e.AddTrait<BTrait>();
            var logic = e.AddControl<ABJobControl>();

            Env.UpdateWorld(1);
            Assert.IsNotNull(logic.Filter.Target);
            Assert.IsNotNull(logic.Filter.Target.GetTrait<ATrait>());
            Assert.IsNotNull(logic.Filter.Target.GetTrait(typeof(BTrait)));

            e.RemoveTrait<BTrait>();
            Env.UpdateWorld(1);
            Assert.IsNull(logic.Filter.Target);
        }

        [TestMethod]
        public void TestVerifyTraitChanged()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var a = e.AddTrait<ATrait>();
            var b = e.AddTrait<BTrait>();
            var logic = e.AddControl<ABReactControl>();

            Assert.IsNotNull(logic.Filter.Target);
            Env.UpdateWorld(1);
            Assert.IsNull(logic.Filter.Target);

            a.ChangeValue();
            Assert.IsNotNull(logic.Filter.Target);
            Env.UpdateWorld(1);

            b.ChangeValue();
            Assert.IsNotNull(logic.Filter.Target);
            Env.UpdateWorld(1);

            a.ChangeValue();
            b.ChangeValue();
            Assert.IsNotNull(logic.Filter.Target);
            Env.UpdateWorld(1);
            Assert.IsNull(logic.Filter.Target);
        }

        [TestMethod]
        public void TestVerifyMatcher()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
            var a = e.AddTrait<ATrait>();
            var b = e.AddTrait<BTrait>();
            e.AddTag("tag");
            var l1 = e.AddControl<ABJobControl>();
            var l2 = e.AddControl<AWithoutBJobControl>();
            var l3 = e.AddControl<CTagJobControl>();

            Assert.IsNotNull(l1.Filter.Target);
            Assert.AreEqual(l1.Filter.Target, e);
            Assert.IsNull(l2.Filter.Target);
            Assert.IsNull(l3.Filter.Target);

            e.RemoveTrait<BTrait>();
            Assert.IsNull(l1.Filter.Target);
            Assert.IsNotNull(l2.Filter.Target);
            Assert.AreEqual(l2.Filter.Target, e);
            Assert.IsNull(l3.Filter.Target);

            e.AddTag("C");
            Assert.IsNull(l1.Filter.Target);
            Assert.IsNotNull(l2.Filter.Target);
            Assert.AreEqual(l2.Filter.Target, e);
            Assert.IsNotNull(l3.Filter.Target);

            e.RemoveTag("C");
            Assert.IsNull(l1.Filter.Target);
            Assert.IsNotNull(l2.Filter.Target);
            Assert.AreEqual(l2.Filter.Target, e);
            Assert.IsNull(l3.Filter.Target);
        }

        [TestMethod]
        public void TestVerifyTraitDispose()
        {
            var world = Env.GetWorld();
            var e = world.CreateEntity();
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

            var e = world.CreateEntity();
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

            var e0 = world.CreateEntity();
            e0.AddTrait<ATrait>();

            Env.UpdateWorld(1);

            var id0 = e0.Id;
            e0.Dispose();

            var e1 = world.CreateEntity();
            Assert.AreNotEqual(id0, e1.Id);

            Env.UpdateWorld(1);

            var e2 = world.CreateEntity();
            Assert.AreEqual(e0, e2);    // reuse the same entity, but all content is clean
            Assert.AreNotEqual(id0, e2.Id);
            Assert.AreNotEqual(e1.Id, e2.Id);
            Assert.IsNull(e2.GetTrait<ATrait>());
        }
    }
}
