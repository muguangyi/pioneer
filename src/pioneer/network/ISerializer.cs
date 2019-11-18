/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Buffer;

namespace Pioneer.Network
{
    /// <summary>
    /// Network packet serializer.
    /// </summary>
    interface ISerializer
    {
        /// <summary>
        /// Marshal object into buf stream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        bool Marshal(object obj, IBufWriter writer);

        /// <summary>
        /// Unmarshal buf stream to target object.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Unmarshal(IBufReader reader, out object obj);
    }
}
