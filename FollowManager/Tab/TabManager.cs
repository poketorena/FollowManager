using System.Collections.ObjectModel;

namespace FollowManager.Tab
{
    /// <summary>
    /// タブを管理するオブジェクト
    /// </summary>
    public class TabManager
    {
        /// <summary>
        /// タブのコレクション
        /// </summary>
        public ObservableCollection<TabData> TabDatas { get; set; } = new ObservableCollection<TabData>();
    }
}
