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
    public class WorldTests
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
        public void TestCreatWorld()
        {
            var world = Env.GetWorld();
            Assert.IsNotNull(world);
        }

        [TestMethod]
        public void TestCreateActor()
        {
            var world = Env.GetWorld();
            var actor = world.CreateActor();
            Assert.IsNotNull(actor);
        }

        [TestMethod]
        public void TestDestroyActor()
        {
            var world = Env.GetWorld();
            var actor = world.CreateActor();
            actor.Dispose();
            Env.UpdateWorld(1);
            Assert.IsNull(world.GetActorById(actor.Id));
        }

        [TestMethod]
        public void TestGetActor()
        {
            var world = Env.GetWorld();
            var e1 = world.CreateActor();
            var id = e1.Id;
            var e2 = world.GetActorById(id);
            Assert.IsNotNull(e2);
            Assert.AreEqual(e1, e2);
            Assert.AreEqual(e1.Id, e2.Id);
        }

        [TestMethod]
        public void TestIterateEntities()
        {
            var world = Env.GetWorld();
            world.CreateActor();
            world.CreateActor();
            world.CreateActor();

            var entities = world.Actors;
            Assert.AreEqual(3, entities.Count());
        }

        [TestMethod]
        public void TestAddSystem()
        {
            var world = Env.GetWorld();
            Assert.IsNotNull(world.AddSystem<NoneSystem>());
        }

        [TestMethod]
        public void TestAddSystem2()
        {
            var world = Env.GetWorld();
            Assert.IsNotNull(world.AddSystem(typeof(NoneSystem)));
        }

        [TestMethod]
        public void TestAddSystem3()
        {
            var world = Env.GetWorld();
            Assert.IsNotNull(world.AddSystem("Pioneer.Test.Pioneer.Support.NoneSystem"));
        }

        [TestMethod]
        public void TestCantCreateMultipleSameSystems()
        {
            var world = Env.GetWorld();
            Assert.IsNotNull(world.AddSystem<NoneSystem>());
            Assert.IsNull(world.AddSystem(typeof(NoneSystem)));
            Assert.IsNull(world.AddSystem("Pioneer.Test.Pioneer.Support.NoneSystem"));
        }
    }
}
