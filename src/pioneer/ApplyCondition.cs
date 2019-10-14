/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer
{
    /// <summary>
    /// Sync apply condition.
    /// </summary>
    public enum ApplyCondition
    {
        /// <summary>
        /// Apply to all clients.
        /// </summary>
        All,

        /// <summary>
        /// Apply only to owner client.
        /// </summary>
        OwnerOnly,

        /// <summary>
        /// Apply except owner clinet.
        /// </summary>
        SkipOwner,
    }
}
