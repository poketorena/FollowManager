using System.Collections.ObjectModel;
using System.Linq;
using Dragablz;
using FollowManager.Collections.ObjectModel.Extensions;
using FollowManager.EventAggregator;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Events;
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

        // パブリック関数

        /// <summary>
        /// クリックでタブを閉じるときに呼ばれるメソッド
        /// </summary>
        public void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> itemActionCallbackArgs)
        {
            var tabData = (TabData)itemActionCallbackArgs.DragablzItem.DataContext;

            var tabRemovedEventArgs = new TabRemovedEventArgs { TabId = tabData.TabId };

            _eventAggregator.GetEvent<TabRemovedEvent>().Publish(tabRemovedEventArgs);
        }

        /// <summary>
        /// タブを閉じるメソッド
        /// </summary>
        /// <param name="tabData">タブのデータ</param>
        public void CloseTab(TabData tabData)
        {
            var tabRemovedEventArgs = new TabRemovedEventArgs { TabId = tabData.TabId };
            _eventAggregator.GetEvent<TabRemovedEvent>().Publish(tabRemovedEventArgs);
            TabDatas.Remove(tabData);
        }

        /// <summary>
        /// 全てのタブを閉じるメソッド
        /// </summary>
        /// <param name="tabData"></param>
        public void CloseAllTabs()
        {
            foreach (var tabData in TabDatas)
            {
                var tabRemovedEventArgs = new TabRemovedEventArgs { TabId = tabData.TabId };
                _eventAggregator.GetEvent<TabRemovedEvent>().Publish(tabRemovedEventArgs);
            }
            TabDatas.Clear();
        }

        /// <summary>
        /// このタブ以外すべてを閉じるメソッド
        /// </summary>
        /// <param name="thisTabData">残しておくタブ</param>
        public void CloseAllTabsExceptThisTab(TabData thisTabData)
        {
            TabDatas
                .Where(tabData => tabData.TabId != thisTabData.TabId)
                .ForEach(eventTabData =>
                {
                    var tabRemovedEventArgs = new TabRemovedEventArgs { TabId = eventTabData.TabId };
                    _eventAggregator.GetEvent<TabRemovedEvent>().Publish(tabRemovedEventArgs);
                });

            TabDatas.RemoveAll(tabData => tabData.TabId != thisTabData.TabId);
        }

        // プライベート変数

        private ObservableCollection<TabData> _tabDatas;

        // DI注入される変数

        private readonly IEventAggregator _eventAggregator;

        // コンストラクタ

        public TabManager(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}
