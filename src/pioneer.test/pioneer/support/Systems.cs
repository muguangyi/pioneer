/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer.Test.Pioneer.Support
{
    public abstract class BaseSystem : System
    {
        public override void Dispose()
        {
            --SystemCount;
        }

        public override void OnInit(IWorld world)
        {
            ++SystemCount;
        }

        public override void OnUpdate(IWorld world, float deltaTime)
        {
        }

        public static int SystemCount = 0;
    }

    public class NoneSystem : BaseSystem
    {
    }

    public class ABJobSystem : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            base.OnInit(world);

            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = world.GetFilter(this, TupleType.Job, this.Matcher);
        }
    }

    public class ABReactSystem : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;
        public int ReactCount = 0;

        public override void OnInit(IWorld world)
        {
            base.OnInit(world);

            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = world.GetFilter(this, TupleType.Reactive, this.Matcher);
        }

        public override void OnUpdate(IWorld world, float deltaTime)
        {
            this.ReactCount = this.Filter.Count;
        }
    }

    public class ABModifySystem : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;
        public int ModifyCount = 0;

        public override void OnInit(IWorld world)
        {
            base.OnInit(world);

            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = world.GetFilter(this, TupleType.Job, this.Matcher);
        }

        public override void OnUpdate(IWorld world, float deltaTime)
        {
            this.ModifyCount = 0;
            var rnd = new Random();

            foreach (var e in this.Filter.Target)
            {
                var a = e.GetTrait<ATrait>();
                a.ChangeValue();
                ++this.ModifyCount;

                if (rnd.NextDouble() < 0.5)
                {
                    break;
                }
            }
        }
    }

    public class ABJobDeleteSystem : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            base.OnInit(world);

            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>().HasTrait<BTrait>();
            this.Filter = world.GetFilter(this, TupleType.Job, this.Matcher);
        }

        public override void OnUpdate(IWorld world, float deltaTime)
        {
            foreach (var e in this.Filter.Target)
            {
                e.Dispose();
                break;
            }
        }
    }

    public class MatcherTestSystem : BaseSystem
    {
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            base.OnInit(world);

            var e = world.CreateActor();

            var matcher = world.NewMatcher();
            matcher.HasTrait<ATrait>()
                   .ExceptTrait<BTrait>()
                   .HasTrait<CTrait>()
                   .ExceptTrait<DTrait>()
                   .HasTag("E")
                   .ExceptTag("F");

            this.Filter = world.GetFilter(this, TupleType.Job, matcher);
        }
    }

    public class ModifyFilterSystem : BaseSystem
    {
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            base.OnInit(world);

            var e = world.CreateActor();

            var matcher = world.NewMatcher();
            matcher.HasTrait<ATrait>().ExceptTrait<BTrait>();

            this.Filter = world.GetFilter(this, TupleType.Job, matcher);
        }

        public override void OnUpdate(IWorld world, float deltaTime)
        {
            foreach (var e in this.Filter.Target)
            {
                e.AddTrait<BTrait>();
            }
        }
    }

    public class SameJobMatcherSystem1 : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>();
            this.Filter = world.GetFilter(this, TupleType.Job, this.Matcher);
        }
    }

    public class SameJobMatcherSystem2 : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>();
            this.Filter = world.GetFilter(this, TupleType.Job, this.Matcher);
        }
    }

    public class SameReactMatcherSystem1 : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>();
            this.Filter = world.GetFilter(this, TupleType.Reactive, this.Matcher);
        }
    }

    public class SameReactMatcherSystem2 : BaseSystem
    {
        public IMatcher Matcher = null;
        public IActorsFilter Filter = null;

        public override void OnInit(IWorld world)
        {
            this.Matcher = world.NewMatcher();
            this.Matcher.HasTrait<ATrait>();
            this.Filter = world.GetFilter(this, TupleType.Reactive, this.Matcher);
        }
    }
}
