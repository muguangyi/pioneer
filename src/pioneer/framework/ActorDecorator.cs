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

namespace Pioneer.Framework
{
    sealed class ActorDecorator : IActorDecorator, IDisposable
    {
        private Dictionary<string, Action<IActor>> templates = new Dictionary<string, Action<IActor>>();

        public void Dispose()
        { }

        public bool TrySetTemplate(string template, Action<IActor> decorator)
        {
            if (this.templates.ContainsKey(template))
            {
                return false;
            }

            this.templates.Add(template, decorator);
            return true;
        }

        public void Apply(string template, IActor actor)
        {
            if (string.IsNullOrEmpty(template))
            {
                return;
            }

            if (this.templates.TryGetValue(template, out Action<IActor> decorator))
            {
                decorator(actor);
            }
        }
    }
}
