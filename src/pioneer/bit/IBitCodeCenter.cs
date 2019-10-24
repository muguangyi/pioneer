/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer.Bit
{
    /// <summary>
    /// BitCode center interface.
    /// </summary>
    interface IBitCodeCenter
    {
        /// <summary>
        /// Get bit code by type.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <returns>BitCode instance.</returns>
        BitCode GetBitCodeByType(Type type);

        /// <summary>
        /// Get bit code by string.
        /// </summary>
        /// <param name="token">Token string.</param>
        /// <returns>BitCode instance.</returns>
        BitCode GetBitCodeByString(string token);
    }
}
