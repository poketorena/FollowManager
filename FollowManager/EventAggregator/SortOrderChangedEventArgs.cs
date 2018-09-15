using FollowManager.FilterAndSort;
using FollowManager.Tab;

namespace FollowManager.EventAggregator
{
    public class SortOrderChangedEventArgs : ISidePanelChangedEventArgs
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
