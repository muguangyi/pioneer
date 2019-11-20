/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Base;
using PP = Pioneer.Pioneer;

namespace Pioneer.Test.Pioneer.Support
{
    static class Env
    {
        public static void Start()
        {
            InstantObject.Reset();
            world = PP.New();
        }

        public static void Stop()
        {
            if (null != world)
            {
                world.Dispose();
                world = null;
            }
        }

        public static IWorld GetWorld()
        {
            return world;
        }

        public static void UpdateWorld(uint times)
        {
            for (var i = 0; i < times; ++i)
            {
                world.Update(0.01f);
            }
        }

        private static IWorld world = null;
    }
}
