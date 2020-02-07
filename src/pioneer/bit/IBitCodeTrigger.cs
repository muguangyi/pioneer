/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Framework;

namespace Pioneer.Bit
{
    /// <summary>
    /// BitCode trigger interface.
    /// </summary>
    interface IBitCodeTrigger
    {
        /// <summary>
        /// When BitCode target initialize.
        /// </summary>
        /// <param name="target">Actor instance.</param>
        void OnBitCodeTargetInit(Actor target);

        /// <summary>
        /// When BitCode target added a BitCode instance.
        /// </summary>
        /// <param name="target">Actor instance.</param>
        /// <param name="code">BitCode instance.</param>
        void OnBitCodeTargetAdded(Actor target, BitCode code);

        /// <summary>
        /// When BitCode target removed a BitCode instance.
        /// </summary>
        /// <param name="target">Actor instance.</param>
        /// <param name="code">BitCode instance.</param>
        void OnBitCodeTargetRemoved(Actor target, BitCode code);

        /// <summary>
        /// When BitCode target modified a BitCode instance.
        /// </summary>
        /// <param name="target">Actor instance.</param>
        /// <param name="code">BitCode instance.</param>
        void OnBitCodeTargetChanged(Actor target, BitCode code);
    }
}
