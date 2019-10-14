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
    /// Abstrace base control component.
    /// </summary>
    public abstract class Control : PureLogic, IControl
    {
        /// <inheritdoc/>
        public virtual void Dispose()
        { }

        /// <inheritdoc/>
        public virtual void OnInit(ITraitContainer container)
        { }

        /// <inheritdoc/>
        public virtual void OnUpdate(ITraitContainer container, float deltaTime)
        { }
    }
}
