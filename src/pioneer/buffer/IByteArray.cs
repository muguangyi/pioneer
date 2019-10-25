/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.IO;

namespace Pioneer.Buffer
{
    interface IByteArray : IDisposable
    {
        byte[] Buffer { get; }

        int Offset { get; }

        int Size { get; }

        int Position { get; }
        
        void Retain();

        void Release();

        void Submit();

        void Seek(int offset = 0, SeekOrigin seekOrigin = SeekOrigin.Begin);

        void SetSize(int size);
    }
}