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
    /// Entity filter interface.
    /// </summary>
    public interface IEntityFilter
    {
        /// <summary>
        /// Gets a value to indicate the owner container.
        /// </summary>
        ITraitContainer Target { get; }
    }
}
