using System;
using System.Linq;
using FollowManager.Service;

namespace FollowManager.Tab
{
    public class AddAccountTabModel
    {
        // パブリック変数

        /// <summary>
        /// 新しいアカウントタブを呼び出し元ウィンドウに追加します。
        /// </summary>
        /// <param name="accounts"></param>
        public void AddAccountTab(object[] accounts)
        {
            var account = (Account.Account)accounts.FirstOrDefault();

            var tabData = new TabData { Header = account.Tokens.ScreenName, TabId = Guid.NewGuid().ToString(), Tokens = account.Tokens };

            _tabManager.TabDatas.Add(tabData);

            _dialogService.CloseAddAccountTabView();
        }

        // DI注入される変数

        private readonly TabManager _tabManager;

        private readonly DialogService _dialogService;

        // コンストラクタ

        public AddAccountTabModel(TabManager tabManager, DialogService dialogService)
        {
            _tabManager = tabManager;
            _dialogService = dialogService;
        }
    }
}
