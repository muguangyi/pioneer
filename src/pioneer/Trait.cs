/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Pioneer
{
    public abstract class Trait : ITrait
    {
        private readonly string typeName = null;
        private Dictionary<string, AbstractMeta> props = new Dictionary<string, AbstractMeta>();
        private Dictionary<string, AbstractArgs> funcs = new Dictionary<string, AbstractArgs>();

        internal Entity Entity { private get; set; }

        /// <inheritdoc />
        public event Action<ITrait> OnTraitChanged;

        /// <inheritdoc />
        public virtual void Dispose()
        { }

        /// <inheritdoc />
        public void OnInit()
        {
        }
    }
}
