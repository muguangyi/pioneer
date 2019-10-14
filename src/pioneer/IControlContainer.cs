/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer
{
    /// <summary>
    /// Control container interface.
    /// </summary>
    public interface IControlContainer : IContainer
    {
        /// <summary>
        /// Add target type control component.
        /// </summary>
        /// <param name="controlType">Control type.</param>
        /// <returns>Control instance.</returns>
        Control AddControl(Type controlType);

        /// <summary>
        /// Add target type control component.
        /// </summary>
        /// <typeparam name="TControl">Target control class that inherits from Control base class.</typeparam>
        /// <returns>Control instance.</returns>
        TControl AddControl<TControl>() where TControl : Control;

        /// <summary>
        /// Add target type control component.
        /// </summary>
        /// <param name="controlName">Target control name.</param>
        /// <returns>Control instance.</returns>
        Control AddControl(string controlName);
    }
}
