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

        public override void OnInit(ITraitContainer container)
        {
            ++ControlCount;
        }

        public override void OnUpdate(ITraitContainer container, float deltaTime)
        {
        }

        public static int ControlCount = 0;
    }

    public class ABJobControl : BaseControl
    {
        public IMatcher Coder = null;
        public IActorFilter Filter = null;

        public override void OnInit(ITraitContainer container)
        {
            base.OnInit(container);

            this.Coder = container.NewMatcher();
            this.Coder.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = container.GetFilter(this, TupleType.Job, this.Coder);
        }
    }

    public class AWithoutBJobControl : BaseControl
    {
        public IMatcher Coder = null;
        public IActorFilter Filter = null;

        public override void OnInit(ITraitContainer container)
        {
            base.OnInit(container);

            this.Coder = container.NewMatcher();
            this.Coder.HasTrait<ATrait>().ExceptTrait<BTrait>();
            this.Filter = container.GetFilter(this, TupleType.Job, this.Coder);
        }
    }

    public class ABReactControl : BaseControl
    {
        public IMatcher Coder = null;
        public IActorFilter Filter = null;

        public override void OnInit(ITraitContainer container)
        {
            base.OnInit(container);

            this.Coder = container.NewMatcher();
            this.Coder.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = container.GetFilter(this, TupleType.Reactive, this.Coder);
        }
    }

    public class CTagJobControl : BaseControl
    {
        public IActorFilter Filter = null;

        public override void OnInit(ITraitContainer container)
        {
            base.OnInit(container);

            var matcher = container.NewMatcher();
            matcher.HasTag("C");
            this.Filter = container.GetFilter(this, TupleType.Job, matcher);
        }
    }
}
