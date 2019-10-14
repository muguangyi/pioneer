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
    /// Entity creator interface.
    /// </summary>
    public interface IEntityCreator
    {
        /// <summary>
        /// Create a new entity.
        /// </summary>
        /// <param name="replicated">Whether the entity needs to be replicated.</param>
        /// <param name="template">Entity template name.</param>
        /// <returns>Entity instance.</returns>
        IEntity CreateEntity(bool replicated = true, string template = null);
    }
}
