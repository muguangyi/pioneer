/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer.Buffer
{
    interface IBufSlice : IBufReader, IBufWriter, IDisposable
    {
        byte[] Buffer { get; }

        int Offset { get; }
        
        void Retain();

        void Release();

        void Submit();

        void SetSize(int size);
    }
}