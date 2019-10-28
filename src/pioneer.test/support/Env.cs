/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer.Test.Support
{
    static class Env
    {
        public static void Start()
        {
            world = Pioneer.New();
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
