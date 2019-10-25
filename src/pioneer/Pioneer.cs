/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer
{
    /// <summary>
    /// Pioneer module entry.
    /// </summary>
    public static class Pioneer
    {
        /// <summary>
        /// New a world instance.
        /// </summary>
        /// <param name="worldMode">World running mode.</param>
        /// <returns>IWorld instance.</returns>
        public static IWorld New(WorldMode worldMode = WorldMode.Standalone)
        {
            return new World(worldMode);
        }
    }
}
