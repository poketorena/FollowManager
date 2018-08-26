using FollowManager.Account;
using FollowManager.FilterAndSort;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowManager.SidePanel
{
    public class SidePanelViewModel : BindableBase
    {
        // プロパティ

        // パブリック関数

        // デリゲートコマンド
        private DelegateCommand<string> _changeFilterCommand;
        public DelegateCommand<string> ChangeFilterCommand =>
            _changeFilterCommand ?? (_changeFilterCommand = new DelegateCommand<string>(_sidePanelModel.ExecuteChangeFilterCommand));

        // インタラクションリクエスト

        // プライベート変数

        // DI注入される変数
        private readonly AccountManager _accountManager;
        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ
        public SidePanelViewModel(AccountManager accountManager, SidePanelModel sidePanelModel)
        {
            _accountManager = accountManager;
            _sidePanelModel = sidePanelModel;
        }

        // デストラクタ

        // プライベート関数
    }
}
