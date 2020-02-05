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
    /// Actor binder interface.
    /// </summary>
    public interface IActorBinder
    {
        /// <summary>
        /// Create a new matcher.
        /// </summary>
        /// <returns>Matcher instance.</returns>
        IMatcher NewMatcher();

        /// <summary>
        /// Get the target filter.
        /// </summary>
        /// <param name="control">Control component.</param>
        /// <param name="tupleType">Tuple type.</param>
        /// <param name="matcher">Matcher instance.</param>
        /// <returns>Filter instance.</returns>
        IActorFilter GetFilter(IControl control, TupleType tupleType, IMatcher matcher);
    }
}
