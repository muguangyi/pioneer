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
    public abstract class PureLogic
    {
        public PureLogic()
        {
            var attrs = GetType().GetCustomAttributes(typeof(PClassAttribute), false);
            this.Domain = (attrs.Length > 0 ? ((PClassAttribute)attrs[0]).Domain : ApplyDomain.NetMultiple);
        }

        private ApplyDomain Domain { get; set; }

        internal bool IsApplied(GameMode mode)
        {
            return ((GameMode.Standalone == mode) ||
                    (ApplyDomain.NetMultiple == this.Domain) ||
                    (ApplyDomain.Client == this.Domain && GameMode.Client == mode) ||
                    (ApplyDomain.Server == this.Domain && GameMode.Server == mode));
        }
    }
}
