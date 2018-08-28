using Prism.Mvvm;

namespace FollowManager.FilterAndSort
{
    /// <summary>
    /// フィルタとソートの設定
    /// </summary>
    public class FilterAndSortOption : BindableBase
    {
        // プロパティ

        /// <summary>
        /// 現在使用しているフィルタ
        /// </summary>
        public FilterType FilterType
        {
            get { return _filterType; }
            set { SetProperty(ref _filterType, value); }
        }

        /// <summary>
        /// 現在使用しているソートキー
        /// </summary>
        public SortKeyType SortKeyType
        {
            get { return _sortKeyType; }
            set { SetProperty(ref _sortKeyType, value); }
        }

        /// <summary>
        /// 現在使用しているソート順
        /// </summary>
        public SortOrderType SortOrderType
        {
            get { return _sortOrderType; }
            set { SetProperty(ref _sortOrderType, value); }
        }

        // プライベート変数

        private FilterType _filterType;

        private SortKeyType _sortKeyType;

        private SortOrderType _sortOrderType;

        // コンストラクタ

        public FilterAndSortOption()
        {
            FilterType = FilterType.Fan;
            SortKeyType = SortKeyType.FollowDay;
            SortOrderType = SortOrderType.Descending;
        }
    }
}
