/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Base;
using Pioneer.Bit;
using Pioneer.Filter;
using Pioneer.Sync;
using Pioneer.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pioneer.Framework
{
    sealed class Actor : ReusableObject, IActor, IBitCodeTrigger
    {
        private World world = null;
        private IBitCodeCenter center = null;
        private Trait[] traits = new Trait[BitCode.MAXSIZE];
        private Dictionary<uint, string> tags = new Dictionary<uint, string>();
        private Filters<ActorFilter> filters = new Filters<ActorFilter>();
        private List<Control> controls = new List<Control>();

        public Actor(World world, IBitCodeCenter center)
        {
            this.world = world;
            this.center = center;
        }

        public ICreator Creator { get; private set; } = null;

        public IWorld World { get => this.world; }

        public ulong Id { get; private set; } = 0;

        public string Template { get; private set; } = string.Empty;

        public override void Dispose()
        {
            this.world.DestroyActor(this);

            base.Dispose();
        }

        public void Active(ICreator owner, ulong id, string template = null)
        {
            base.Active();

            this.Creator = owner;
            this.Id = id;
            this.Template = template;
            this.CompositeCode = BitCode.CompositeCode.Create();
            this.tags.Clear();
            this.controls.Clear();
            this.filters.Active();
        }

        public override void Deactive()
        {
            for (var i = 0; i < this.controls.Count; ++i)
            {
                this.controls[i].Dispose();
            }

            var indices = this.CompositeCode.Indices.ToArray();
            for (var i = 0; i < indices.Length; ++i)
            {
                var index = indices[i];
                BitCode code;
                var trait = this.traits[index];
                if (null != trait)
                {
                    code = this.center.GetBitCodeByType(trait.GetType());
                    trait.OnChanged -= OnTraitChanged;

                    this.traits[index] = null;
                    this.world.DropTrait(trait);
                }
                else
                {
                    code = this.center.GetBitCodeByString(this.tags[index]);
                }

                this.CompositeCode.Subtract(code);
                OnBitCodeTargetRemoved(this, code);
            }

            this.filters.Deactive();

            base.Deactive();
        }

        public void OnUpdate(float deltaTime)
        {
            IControl control = null;
            for (var i = 0; i < this.controls.Count; ++i)
            {
                control = this.controls[i];
                this.filters.OnPreHandling(control);
                {
                    control.OnUpdate(this, deltaTime);
                }
                this.filters.OnPostHandling(control);
            }
        }

        public Trait AddTrait(Type traitType)
        {
            Trait trait = AddTraitInternal(traitType);
            if (this.Replicated && null != trait && WorldMode.Client != this.world.Mode)
            {
                this.world.Do(((Creator)this.Creator).Id, SyncType.Create, SyncTarget.Trait, this.Id, traitType.FullName);
            }

            return trait;
        }

        public TTrait AddTrait<TTrait>() where TTrait : Trait
        {
            var trait = AddTrait(typeof(TTrait));
            return (null != trait ? (TTrait)trait : default(TTrait));
        }

        public Trait AddTrait(string traitName)
        {
            return AddTrait(TypeUtility.GetType(traitName));
        }

        public bool RemoveTrait(Type traitType)
        {
            if (RemoveTraitInternal(traitType) && this.Replicated && WorldMode.Client != this.world.Mode)
            {
                this.world.Do(((Creator)this.Creator).Id, SyncType.Destroy, SyncTarget.Trait, this.Id, traitType.FullName);

                return true;
            }

            return false;
        }

        public bool RemoveTrait<TTrait>() where TTrait : Trait
        {
            return RemoveTrait(typeof(TTrait));
        }

        public bool RemoveTrait(string traitName)
        {
            return RemoveTrait(TypeUtility.GetType(traitName));
        }

        public bool ReplaceTrait(Type srcType, Type destType)
        {
            RemoveTrait(srcType);
            return (null != AddTrait(destType));
        }

        public bool ReplaceTrait<TSrcTrait, TDestTrait>() where TSrcTrait : Trait where TDestTrait : Trait
        {
            return ReplaceTrait(typeof(TSrcTrait), typeof(TDestTrait));
        }

        public bool ReplaceTrait(string srcName, string destName)
        {
            return ReplaceTrait(TypeUtility.GetType(srcName), TypeUtility.GetType(destName));
        }

        public Trait GetTrait(Type traitType)
        {
            if (typeof(Trait).IsAssignableFrom(traitType))
            {
                var code = this.center.GetBitCodeByType(traitType);
                return this.traits[code.Index];
            }

            return null;
        }

        public TTrait GetTrait<TTrait>() where TTrait : Trait
        {
            var trait = GetTrait(typeof(TTrait));
            return (null != trait ? (TTrait)trait : default(TTrait));
        }

        public Trait GetTrait(string traitName)
        {
            return GetTrait(TypeUtility.GetType(traitName));
        }

        public bool HasTrait(Type traitType)
        {
            return (null != GetTrait(traitType));
        }

        public bool HasTrait<TTrait>() where TTrait : Trait
        {
            return HasTrait(typeof(TTrait));
        }

        public bool HasTrait(string traitName)
        {
            return HasTrait(TypeUtility.GetType(traitName));
        }

        public bool TryGetTrait(Type traitType, out Trait trait)
        {
            trait = GetTrait(traitType);
            return (null != trait);
        }

        public bool TryGetTrait<TTrait>(out TTrait trait) where TTrait : Trait
        {
            trait = GetTrait<TTrait>();
            return (default(TTrait) != trait);
        }

        public bool TryGetTrait(string traitName, out Trait trait)
        {
            trait = GetTrait(traitName);
            return (null != trait);
        }

        public bool AddTag(string tag)
        {
            if (AddTagInternal(tag) && this.Replicated && WorldMode.Client != this.world.Mode)
            {
                this.world.Do(((Creator)this.Creator).Id, SyncType.Create, SyncTarget.Tag, this.Id, tag);

                return true;
            }

            return false;
        }

        public bool RemoveTag(string tag)
        {
            if (RemoveTagInternal(tag) && this.Replicated && WorldMode.Client != this.world.Mode)
            {
                this.world.Do(((Creator)this.Creator).Id, SyncType.Destroy, SyncTarget.Tag, this.Id, tag);

                return true;
            }

            return false;
        }

        public bool ReplaceTag(string srcTag, string destTag)
        {
            RemoveTag(srcTag);
            return AddTag(destTag);
        }

        public bool HasTag(string tag)
        {
            var code = this.center.GetBitCodeByString(tag);
            return this.CompositeCode.Contains(code);
        }

        public Control AddControl(Type controlType)
        {
            return AddControlInternal(controlType);
        }

        public TControl AddControl<TControl>() where TControl : Control
        {
            var control = AddControl(typeof(TControl));
            return (null != control ? (TControl)control : default(TControl));
        }

        public Control AddControl(string controlName)
        {
            return AddControl(TypeUtility.GetType(controlName));
        }

        public IMatcher NewMatcher()
        {
            return new Matcher(this.center);
        }

        public IActorFilter GetFilter(IControl control, TupleType tupleType, IMatcher matcher)
        {
            var m = matcher as Matcher;

            ActorFilter filter = this.filters.GetFilter(control, tupleType, m);
            if (null == filter)
            {
                filter = this.filters.AddFilter
                (
                    control,
                    tupleType,
                    TupleType.Job == tupleType ? new JobActorFilter(m) as ActorFilter : new ReactActorFilter(m)
                );
                filter.OnBitCodeTargetInit(this);
            }

            return filter;
        }

        public void OnBitCodeTargetInit(Actor target)
        { }

        public void OnBitCodeTargetAdded(Actor target, BitCode code)
        {
            this.filters.OnBitCodeTargetAdded(target, code);
            this.world.OnBitCodeTargetAdded(target, code);
        }

        public void OnBitCodeTargetRemoved(Actor target, BitCode code)
        {
            this.filters.OnBitCodeTargetRemoved(target, code);
            this.world.OnBitCodeTargetRemoved(target, code);
        }

        public void OnBitCodeTargetChanged(Actor target, BitCode code)
        {
            this.filters.OnBitCodeTargetChanged(target, code);
            this.world.OnBitCodeTargetChanged(target, code);
        }

        internal void DisposeImmediate()
        {
            Deactive();

            this.filters.Dispose();

            this.world = null;
            this.center = null;
            this.traits = null;
            this.controls = null;
            this.filters = null;
        }

        internal void TakeSnapshot()
        {
            var ownerId = ((Creator)this.Creator).Id;
            for (var i = 0; i < this.traits.Length; ++i)
            {
                var trait = this.traits[i];
                if (null != trait)
                {
                    var clsName = trait.GetType().FullName;
                    this.world.Do(ownerId, SyncType.Create, SyncTarget.Trait, this.Id, clsName);
                    trait.TakeSnapshot();
                }
            }
            foreach (var t in this.tags)
            {
                this.world.Do(ownerId, SyncType.Create, SyncTarget.Tag, this.Id, t.Value);
            }
        }

        internal Trait AddTraitInternal(Type traitType)
        {
            if (!typeof(Trait).IsAssignableFrom(traitType))
            {
                return null;
            }

            var code = this.center.GetBitCodeByType(traitType);
            var trait = this.traits[code.Index];
            if (null == trait)
            {
                trait = this.world.PickTrait(traitType);
                trait.Entity = this;
                trait.OnInit();
                trait.OnChanged += OnTraitChanged;

                this.traits[code.Index] = trait;
                this.CompositeCode.Add(code);

                OnBitCodeTargetAdded(this, code);
            }

            return trait;
        }

        internal TTrait AddTraitInternal<TTrait>() where TTrait : Trait
        {
            var trait = AddTraitInternal(typeof(TTrait));
            return (null != trait ? (TTrait)trait : default(TTrait));
        }

        internal Trait AddTraitInternal(string traitName)
        {
            return AddTraitInternal(TypeUtility.GetType(traitName));
        }

        internal bool RemoveTraitInternal(Type traitType)
        {
            if (typeof(Trait).IsAssignableFrom(traitType))
            {
                var code = this.center.GetBitCodeByType(traitType);
                var trait = this.traits[code.Index];
                trait.OnChanged -= OnTraitChanged;

                this.traits[code.Index] = null;
                this.world.DropTrait(trait);
                this.CompositeCode.Subtract(code);

                OnBitCodeTargetRemoved(this, code);

                return true;
            }

            return false;
        }

        internal bool RemoveTraitInternal<TTrait>() where TTrait : Trait
        {
            return RemoveTraitInternal(typeof(TTrait));
        }

        internal bool RemoveTraitInternal(string traitName)
        {
            return RemoveTraitInternal(TypeUtility.GetType(traitName));
        }

        internal bool AddTagInternal(string tag)
        {
            if (!HasTag(tag))
            {
                var code = this.center.GetBitCodeByString(tag);
                this.tags.Add(code.Index, tag);
                this.CompositeCode.Add(code);

                OnBitCodeTargetAdded(this, code);

                return true;
            }

            return false;
        }

        internal bool RemoveTagInternal(string tag)
        {
            if (HasTag(tag))
            {
                var code = this.center.GetBitCodeByString(tag);
                this.tags.Remove(code.Index);
                this.CompositeCode.Subtract(code);

                OnBitCodeTargetRemoved(this, code);

                return true;
            }

            return false;
        }

        internal Control AddControlInternal(Type controlType)
        {
            Control control = GetControlInternal(controlType);
            if (typeof(Control).IsAssignableFrom(controlType) && null == control)
            {
                control = Activator.CreateInstance(controlType) as Control;
                if (control.IsApplied(this.world.Mode))
                {
                    AddControl(control);
                }
            }

            return control;
        }

        internal TControl AddControlInternal<TControl>() where TControl : Control
        {
            var control = AddControlInternal(typeof(TControl));
            return (null != control ? (TControl)control : default(TControl));
        }

        internal Control AddControlInternal(string controlName)
        {
            return AddControlInternal(TypeUtility.GetType(controlName));
        }

        internal Control GetControlInternal(Type controlType)
        {
            for (var i = 0; i < this.controls.Count; ++i)
            {
                if (this.controls[i].GetType() == controlType)
                {
                    return this.controls[i];
                }
            }

            return null;
        }

        internal Control GetControlInternal<TControl>() where TControl : Control
        {
            return GetControlInternal(typeof(TControl));
        }

        internal Control GetControlInternal(string controlName)
        {
            return GetControlInternal(TypeUtility.GetType(controlName));
        }

        internal BitCode.CompositeCode CompositeCode { get; private set; }

        internal bool Replicated { get; set; }

        private void OnTraitChanged(ITrait trait)
        {
            var code = this.center.GetBitCodeByType(trait.GetType());
            OnBitCodeTargetChanged(this, code);
        }

        private void AddControl(Control control)
        {
            control.OnInit(this);
            this.controls.Add(control);
        }

        private void RemoveControl(Control control)
        {
            this.controls.Remove(control);
        }
    }
}
