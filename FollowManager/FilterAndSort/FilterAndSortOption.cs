using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace FollowManager.FilterAndSort
{
    public class FilterAndSortOption : BindableBase
    {
        private FilterType _filterType;
        public FilterType FilterType
        {
            get { return _filterType; }
            set { SetProperty(ref _filterType, value); }
        }

        private SortKeyType _sortType;
        public SortKeyType SortKeyType
        {
            get { return _sortType; }
            set { SetProperty(ref _sortType, value); }
        }

        private SortOption _sortOption;
        public SortOption SortOption
        {
            get { return _sortOption; }
            set { SetProperty(ref _sortOption, value); }
        }

        public FilterAndSortOption()
        {
            FilterType = FilterType.Fan;
            SortKeyType = SortKeyType.LastTweetDay;
            SortOption = SortOption.Descending;
        }
    }
}
