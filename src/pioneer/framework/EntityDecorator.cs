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
    sealed class EntityDecorator : IEntityDecorator, IDisposable
    {
        private Dictionary<string, Action<IEntity>> templates = new Dictionary<string, Action<IEntity>>();

        public void Dispose()
        { }

        public bool AddTemplate(string template, Action<IEntity> decorator)
        {
            if (this.templates.ContainsKey(template))
            {
                return false;
            }

            this.templates.Add(template, decorator);
            return true;
        }

        public void Apply(string template, IEntity entity)
        {
            if (string.IsNullOrEmpty(template))
            {
                return;
            }

            if (this.templates.TryGetValue(template, out Action<IEntity> decorator))
            {
                decorator(entity);
            }
        }
    }
}
