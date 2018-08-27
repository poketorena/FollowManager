using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        private SortKeyType _sortKeyType;
        public SortKeyType SortKeyType
        {
            get { return _sortKeyType; }
            set { SetProperty(ref _sortKeyType, value); }
        }

        private SortOrderType _sortOrderType;
        public SortOrderType SortOrderType
        {
            get { return _sortOrderType; }
            set { SetProperty(ref _sortOrderType, value); }
        }

        public FilterAndSortOption()
        {
            FilterType = FilterType.Fan;
            SortKeyType = SortKeyType.FollowDay;
            SortOrderType = SortOrderType.Descending;
        }
    }
}
