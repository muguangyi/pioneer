/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Buffer;

namespace Pioneer
{
    interface ISerializer
    {
        bool Marshal(object obj, IBufWriter writer);
        bool Unmarshal(IBufReader reader, out object obj);
    }
}
