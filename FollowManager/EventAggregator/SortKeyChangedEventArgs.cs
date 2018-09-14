using FollowManager.FilterAndSort;
using FollowManager.Tab;

namespace FollowManager.EventAggregator
{
    /// <summary>
    /// タブのデータとフィルタとソートの設定
    /// </summary>
    public class SortKeyChangedEventArgs : IDataChangedEventArgs
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        public TabData TabData { get; set; }

        /// <summary>
        /// フィルタとソートの設定
        /// </summary>
        public FilterAndSortOption FilterAndSortOption { get; set; }
    }
}