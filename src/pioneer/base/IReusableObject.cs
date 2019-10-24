/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer.Base
{
    /// <summary>
    /// Reusable object interface.
    /// </summary>
    interface IReusableObject
    {
        /// <summary>
        /// Active.
        /// </summary>
        void Active();

        /// <summary>
        /// De-active.
        /// </summary>
        void Deactive();
    }
}
