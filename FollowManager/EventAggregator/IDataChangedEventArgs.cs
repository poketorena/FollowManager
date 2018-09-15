using FollowManager.FilterAndSort;
using FollowManager.Tab;

namespace FollowManager.EventAggregator
{
    /// <summary>
    /// SidePanelで発生したイベントデータ
    /// </summary>
    public interface ISidePanelChangedEventArgs
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        TabData TabData { get; set; }

        /// <summary>
        /// フィルタとソートの設定
        /// </summary>
        FilterAndSortOption FilterAndSortOption { get; set; }
    }
}
