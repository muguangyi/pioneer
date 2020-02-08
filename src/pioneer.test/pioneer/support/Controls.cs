/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer.Test.Pioneer.Support
{
    public class BaseControl : Control
    {
        public override void Dispose()
        {
            --ControlCount;
        }

        public override void OnInit(IActor actor)
        {
            ++ControlCount;
        }

        public override void OnUpdate(IActor actor, float deltaTime)
        {
        }

        public static int ControlCount = 0;
    }

    public class ABJobControl : BaseControl
    {
        public IMatcher Coder = null;
        public IActorFilter Filter = null;

        public override void OnInit(IActor actor)
        {
            base.OnInit(actor);

            this.Coder = actor.NewMatcher();
            this.Coder.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = actor.GetFilter(this, TupleType.Job, this.Coder);
        }
    }

    public class AWithoutBJobControl : BaseControl
    {
        public IMatcher Coder = null;
        public IActorFilter Filter = null;

        public override void OnInit(IActor actor)
        {
            base.OnInit(actor);

            this.Coder = actor.NewMatcher();
            this.Coder.HasTrait<ATrait>().ExceptTrait<BTrait>();
            this.Filter = actor.GetFilter(this, TupleType.Job, this.Coder);
        }
    }

    public class ABReactControl : BaseControl
    {
        public IMatcher Coder = null;
        public IActorFilter Filter = null;

        public override void OnInit(IActor actor)
        {
            base.OnInit(actor);

            this.Coder = actor.NewMatcher();
            this.Coder.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = actor.GetFilter(this, TupleType.Reactive, this.Coder);
        }
    }

    public class CTagJobControl : BaseControl
    {
        public IActorFilter Filter = null;

        public override void OnInit(IActor actor)
        {
            base.OnInit(actor);

            var matcher = actor.NewMatcher();
            matcher.HasTag("C");
            this.Filter = actor.GetFilter(this, TupleType.Job, matcher);
        }
    }
}
