using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace FollowManager.Tab
{
    /// <summary>
    /// タブを管理するオブジェクト
    /// </summary>
    public class TabManager : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのコレクション
        /// </summary>
        public ObservableCollection<TabData> TabDatas
        {
            get { return _tabDatas ?? (_tabDatas = new ObservableCollection<TabData>()); }
            set { SetProperty(ref _tabDatas, value); }
        }

        // プライベート変数

        private ObservableCollection<TabData> _tabDatas;
    }
}
