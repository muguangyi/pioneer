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
    class Creator : InstantObject, ICreator
    {
        private Stack<IActor> entities = new Stack<IActor>();

        public Creator(ulong id, World world)
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
                return (WorldMode.Client != this.World.Mode);
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

        public IActor CreateActor(bool replicated = true, string template = null)
        {
            var e = this.World.CreateActorByOwner(this, replicated, template);
            this.entities.Push(e);

            return e;
        }
    }
}
