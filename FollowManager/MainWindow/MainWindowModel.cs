using Dragablz;

namespace FollowManager.MainWindow
{
    public class MainWindowModel
    {
        // プライベート変数

        /// <summary>
        /// タブを閉じるときに呼ばれるメソッド
        /// </summary>
        public void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            //in here you can dispose stuff or cancel the close

            //here's your view model:
            var viewModel = args.DragablzItem.DataContext as HeaderedItemViewModel;

            //here's how you can cancel stuff:
            //args.Cancel(); 
        }
    }
}
