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
    /// Filter interface.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Gets a value to indicate whether the filter conditions are matched.
        /// </summary>
        bool Matched { get; }
    }
}
