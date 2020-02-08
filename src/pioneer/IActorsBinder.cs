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
    /// Actors' binder interface.
    /// </summary>
    public interface IActorsBinder
    {
        /// <summary>
        /// Create a new matcher.
        /// </summary>
        /// <returns>Matcher instance.</returns>
        IMatcher NewMatcher();

        /// <summary>
        /// Get target filter instance.
        /// </summary>
        /// <param name="system">System component.</param>
        /// <param name="tupleType">Tuple type.</param>
        /// <param name="matcher">Matcher instance.</param>
        /// <returns>Filter instance.</returns>
        IGroupFilter GetFilter(ISystem system, TupleType tupleType, IMatcher matcher);
    }
}
