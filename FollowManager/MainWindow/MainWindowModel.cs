using Dragablz;
using FollowManager.EventAggregator;
using FollowManager.Tab;
using Prism.Events;

namespace FollowManager.MainWindow
{
    public class MainWindowModel
    {
        // パブリック関数

        /// <summary>
        /// タブを閉じるときに呼ばれるメソッド
        /// </summary>
        public void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> itemActionCallbackArgs)
        {
            var tabData = (TabData)itemActionCallbackArgs.DragablzItem.DataContext;

            var tabRemovedEventArgs = new TabRemovedEventArgs { TabId = tabData.TabId };

            _eventAggregator.GetEvent<TabRemovedEvent>().Publish(tabRemovedEventArgs);
        }

        // DI注入される変数

        private readonly IEventAggregator _eventAggregator;

        // コンストラクタ

        public MainWindowModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}
