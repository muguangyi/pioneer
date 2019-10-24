/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer
{
    interface ISerializer
    {
        ArraySegment<byte> Marshal(object obj);
        object Unmarshal(ArraySegment<byte> bytes);
        int Slice(ArraySegment<byte> source);
    }
}
