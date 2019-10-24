/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Base;
using System.Collections.Generic;

namespace Pioneer.Framework
{
    class EntityCreator : InstantObject, IEntityCreator
    {
        private Stack<IEntity> entities = new Stack<IEntity>();

        public EntityCreator(ulong id, World world)
        {
            this.Id = id;
            this.World = world;
        }

        public ulong Id { get; }

        public World World { get; }

        public virtual bool HasAuthority
        {
            get
            {
                return (GameMode.Client != this.World.GameMode);
            }
        }


        public override void Dispose()
        {
            while (this.entities.Count > 0)
            {
                this.entities.Pop().Dispose();
            }

            base.Dispose();
        }

        public IEntity CreateEntity(bool replicated = true, string template = null)
        {
            var e = this.World.CreateEntityByOwner(this, replicated, template);
            this.entities.Push(e);

            return e;
        }
    }
}
