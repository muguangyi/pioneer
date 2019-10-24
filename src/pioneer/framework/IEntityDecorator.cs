/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer.Framework
{
    /// <summary>
    /// Entity decorator interface.
    /// </summary>
    interface IEntityDecorator
    {
        /// <summary>
        /// Apply template to target entity.
        /// </summary>
        /// <param name="template">Template name.</param>
        /// <param name="entity">Target entity instance.</param>
        void Apply(string template, IEntity entity);
    }
}
