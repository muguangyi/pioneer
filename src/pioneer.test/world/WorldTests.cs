/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pioneer.Test.World.Support;
using System.Linq;

namespace Pioneer.Test.World
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
        public void TestCreateEntity()
        {
            var world = Env.GetWorld();
            var entity = world.CreateEntity();
            Assert.IsNotNull(entity);
        }

        [TestMethod]
        public void TestDestroyEntity()
        {
            var world = Env.GetWorld();
            var entity = world.CreateEntity();
            entity.Dispose();
            Env.UpdateWorld(1);
            Assert.IsNull(world.GetEntityById(entity.Id));
        }

        [TestMethod]
        public void TestGetEntity()
        {
            var world = Env.GetWorld();
            var e1 = world.CreateEntity();
            var id = e1.Id;
            var e2 = world.GetEntityById(id);
            Assert.IsNotNull(e2);
            Assert.AreEqual(e1, e2);
            Assert.AreEqual(e1.Id, e2.Id);
        }

        [TestMethod]
        public void TestIterateEntities()
        {
            var world = Env.GetWorld();
            world.CreateEntity();
            world.CreateEntity();
            world.CreateEntity();

            var entities = world.Entities;
            Assert.AreEqual(entities.Count(), 3);
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
            Assert.IsNotNull(world.AddSystem("Pioneer.Test.World.Support.NoneSystem"));
        }

        [TestMethod]
        public void TestCantCreateMultipleSameSystems()
        {
            var world = Env.GetWorld();
            Assert.IsNotNull(world.AddSystem<NoneSystem>());
            Assert.IsNull(world.AddSystem(typeof(NoneSystem)));
            Assert.IsNull(world.AddSystem("Pioneer.Test.World.Support.NoneSystem"));
        }
    }
}
