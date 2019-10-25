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
    /// <summary>
    /// Trait interface.
    /// </summary>
    public interface ITrait : IDisposable
    {
        /// <summary>
        /// Event when a trait changed.
        /// </summary>
        event Action<ITrait> OnChanged;

        /// <summary>
        /// Initialize entrance.
        /// </summary>
        void OnInit();
    }
}
