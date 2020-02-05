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
    /// Actor decorator interface.
    /// </summary>
    interface IActorDecorator
    {
        /// <summary>
        /// Apply template to target actor.
        /// </summary>
        /// <param name="template">Template name.</param>
        /// <param name="actor">Target actor instance.</param>
        void Apply(string template, IActor actor);
    }
}
