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
    /// Abstrace base system component.
    /// </summary>
    public abstract class System : PureLogic, ISystem
    {
        /// <inheritdoc />
        public virtual void Dispose()
        { }

        /// <inheritdoc />
        public virtual void OnInit(IWorld world)
        { }

        /// <inheritdoc />
        public virtual void OnUpdate(IWorld world, float deltaTime)
        { }
    }
}
