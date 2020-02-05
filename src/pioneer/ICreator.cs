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
    /// Actor creator interface.
    /// </summary>
    public interface ICreator
    {
        /// <summary>
        /// Create a new actor.
        /// </summary>
        /// <param name="replicated">Whether the actor needs to be replicated.</param>
        /// <param name="template">Actor template name.</param>
        /// <returns>Actor instance.</returns>
        IActor CreateActor(bool replicated = true, string template = null);
    }
}
