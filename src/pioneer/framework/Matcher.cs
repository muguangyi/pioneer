/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Pioneer.Bit;
using Pioneer.Util;
using System;
using System.Collections.Generic;

namespace Pioneer.Framework
{
    class Matcher : IMatcher
    {
        private IBitCodeCenter center = null;
        private HashSet<uint> indices = new HashSet<uint>();

        public Matcher(IBitCodeCenter center)
        {
            this.center = center;
            this.MatchCode = BitCode.CompositeCode.Create();
            this.ExceptCode = BitCode.CompositeCode.Create();
        }

        public BitCode.CompositeCode MatchCode { get; }

        public BitCode.CompositeCode ExceptCode { get; }

        public IEnumerable<uint> Indices => this.indices;

        public static bool operator ==(Matcher m0, Matcher m1)
        {
            return (m0.MatchCode == m1.MatchCode && m0.ExceptCode == m1.ExceptCode);
        }

        public static bool operator !=(Matcher m0, Matcher m1)
        {
            return !(m0 == m1);
        }

        public override bool Equals(object obj)
        {
            if (obj is Matcher)
            {
                var m = obj as Matcher;
                return (m.MatchCode == this.MatchCode && m.ExceptCode == this.ExceptCode);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public IMatcher HasTrait(Type traitType)
        {
            var code = this.center.GetBitCodeByType(traitType);
            if (this.indices.Add(code.Index))
            {
                this.MatchCode.Add(code);
            }

            return this;
        }

        public IMatcher HasTrait<TTrait>() where TTrait : ITrait
        {
            var code = this.center.GetBitCodeByType(typeof(TTrait));
            if (this.indices.Add(code.Index))
            {
                this.MatchCode.Add(code);
            }

            return this;
        }

        public IMatcher HasTrait(string traitName)
        {
            var code = this.center.GetBitCodeByType(TypeUtility.GetType(traitName));
            if (this.indices.Add(code.Index))
            {
                this.MatchCode.Add(code);
            }

            return this;
        }

        public IMatcher HasTag(string tag)
        {
            var code = this.center.GetBitCodeByString(tag);
            if (this.indices.Add(code.Index))
            {
                this.MatchCode.Add(code);
            }

            return this;
        }

        public IMatcher ExceptTrait(Type traitType)
        {
            var code = this.center.GetBitCodeByType(traitType);
            if (this.indices.Add(code.Index))
            {
                this.ExceptCode.Add(code);
            }

            return this;
        }

        public IMatcher ExceptTrait<TTrait>() where TTrait : ITrait
        {
            var code = this.center.GetBitCodeByType(typeof(TTrait));
            if (this.indices.Add(code.Index))
            {
                this.ExceptCode.Add(code);
            }

            return this;
        }

        public IMatcher ExceptTrait(string traitName)
        {
            var code = this.center.GetBitCodeByType(TypeUtility.GetType(traitName));
            if (this.indices.Add(code.Index))
            {
                this.ExceptCode.Add(code);
            }

            return this;
        }

        public IMatcher ExceptTag(string tag)
        {
            var code = this.center.GetBitCodeByString(tag);
            if (this.indices.Add(code.Index))
            {
                this.ExceptCode.Add(code);
            }

            return this;
        }
    }
}
