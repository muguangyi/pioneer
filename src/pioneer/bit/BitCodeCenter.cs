/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Pioneer
{
    class BitCodeCenter : IBitCodeCenter, IDisposable
    {
        private Dictionary<string, BitCode> uniqueCodes = new Dictionary<string, BitCode>();
        private uint index = 0;

        public void Dispose()
        {
            this.uniqueCodes = null;
        }

        /// <inheritdoc/>
        public BitCode GetBitCodeByType(Type type)
        {
            return GetBitCodeByString(type.FullName);
        }

        /// <inheritdoc/>
        public BitCode GetBitCodeByString(string token)
        {
            if (!this.uniqueCodes.TryGetValue(token, out BitCode code))
            {
                this.uniqueCodes.Add(token, code = new BitCode(this.index++));
            }

            return code;
        }
    }
}
