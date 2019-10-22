/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;

namespace Pioneer
{
    class Filters<TFilter> : ReusableObject, IBitCodeTrigger where TFilter : Filter
    {
        private List<TFilter> sharedFilters = new List<TFilter>();
        private Dictionary<int, List<TFilter>>[] filterGroups = new Dictionary<int, List<TFilter>>[2]
        {
            new Dictionary<int, List<TFilter>>(),
            new Dictionary<int, List<TFilter>>(),
        };
        private LinkedList<TFilter>[] quickFilters = null;

        public override void Dispose()
        {
            Deactive();

            this.sharedFilters = null;
            this.filterGroups = null;
            this.quickFilters = null;

            base.Dispose();
        }

        public override void Active()
        {
            base.Active();

            this.sharedFilters.Clear();
            for (var i = 0; i < 2; ++i)
            {
                this.filterGroups[i].Clear();
            }

            this.quickFilters = new LinkedList<TFilter>[BitCode.MAXSIZE];
        }

        public override void Deactive()
        {
            for (var i = 0; i < this.sharedFilters.Count; ++i)
            {
                this.sharedFilters[i].Dispose();
            }

            var group = GetFilterGroup(TupleType.Reactive);
            foreach (var p in group)
            {
                var filters = p.Value;
                for (var i = 0; i < filters.Count; ++i)
                {
                    filters[i].Dispose();
                }
            }

            base.Deactive();
        }

        public TFilter AddFilter(object handler, TupleType tupleType, TFilter filter)
        {
            var group = GetFilterGroup(tupleType);
            var key = handler.GetHashCode();
            if (!group.TryGetValue(key, out List<TFilter> filters))
            {
                group.Add(key, filters = new List<TFilter>());
            }

            filters.Add(filter);
            if (TupleType.Job == tupleType)
            {
                this.sharedFilters.Add(filter);
            }

            UpdateQuickFilters(filter);

            return filter;
        }

        public TFilter GetFilter(object handler, TupleType tupleType, Matcher matcher)
        {
            var group = GetFilterGroup(tupleType);
            var key = handler.GetHashCode();
            if (!group.TryGetValue(key, out List<TFilter> filters))
            {
                group.Add(key, filters = new List<TFilter>());
            }

            var filter = FindMatchedFilter(filters, matcher);
            if (null == filter && TupleType.Job == tupleType)
            {
                filter = FindMatchedFilter(this.sharedFilters, matcher);
                if (null != filter)
                {
                    filters.Add(filter);
                }
            }

            return filter;
        }

        public void OnBitCodeTargetInit(Entity entity)
        { }

        public void OnBitCodeTargetAdded(Entity target, BitCode code)
        {
            var filters = this.quickFilters[code.Index];
            if (null != filters && filters.Count > 0)
            {
                var filter = filters.First;
                while (null != filter)
                {
                    filter.Value.OnBitCodeTargetAdded(target, code);

                    filter = filter.Next;
                }
            }
        }

        public void OnBitCodeTargetRemoved(Entity target, BitCode code)
        {
            var filters = this.quickFilters[code.Index];
            if (null != filters && filters.Count > 0)
            {
                var filter = filters.First;
                while (null != filter)
                {
                    filter.Value.OnBitCodeTargetRemoved(target, code);

                    filter = filter.Next;
                }
            }
        }

        public void OnBitCodeTargetChanged(Entity target, BitCode code)
        {
            var filters = this.quickFilters[code.Index];
            if (null != filters && filters.Count > 0)
            {
                var filter = filters.First;
                while (null != filter)
                {
                    filter.Value.OnBitCodeTargetChanged(target, code);

                    filter = filter.Next;
                }
            }
        }

        public void OnPreHandling(object handler)
        {
            var key = handler.GetHashCode();
            for (var i = 0; i < 2; ++i)
            {
                if (this.filterGroups[i].TryGetValue(key, out List<TFilter> filters))
                {
                    for (var j = 0; j < filters.Count; ++j)
                    {
                        filters[j].OnPreHandling();
                    }
                }
            }
        }

        public void OnPostHandling(object handler)
        {
            var key = handler.GetHashCode();
            for (var i = 0; i < 2; ++i)
            {
                if (this.filterGroups[i].TryGetValue(key, out List<TFilter> filters))
                {
                    for (var j = 0; j < filters.Count; ++j)
                    {
                        filters[j].OnPostHandling();
                    }
                }
            }
        }

        private Dictionary<int, List<TFilter>> GetFilterGroup(TupleType tupleType)
        {
            return this.filterGroups[(int)tupleType];
        }

        private TFilter FindMatchedFilter(List<TFilter> filters, Matcher matcher)
        {
            for (var i = 0; i < filters.Count; ++i)
            {
                var filter = filters[i];
                if (filter.Matcher == matcher)
                {
                    return filter;
                }
            }

            return default(TFilter);
        }

        private void UpdateQuickFilters(TFilter filter)
        {
            var indices = filter.Matcher.Indices;
            foreach (var index in indices)
            {
                var list = this.quickFilters[index];
                if (null == list)
                {
                    this.quickFilters[index] = list = new LinkedList<TFilter>();
                }

                list.AddLast(filter);
            }
        }
    }
}
