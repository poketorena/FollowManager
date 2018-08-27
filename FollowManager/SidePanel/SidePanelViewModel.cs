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
        private DelegateCommand<string> _changeFilterTypeCommand;
        public DelegateCommand<string> ChangeFilterTypeCommand =>
                                                                                                              // HACK: 非同期処理は要調整
            _changeFilterTypeCommand ?? (_changeFilterTypeCommand = new DelegateCommand<string>(filterType => Task.Run(() => _sidePanelModel.ChangeFilterType(filterType))));

        private DelegateCommand<string> _changeSortKeyTypeCommand;
        public DelegateCommand<string> ChangeSortKeyTypeCommand =>
            _changeSortKeyTypeCommand ?? (_changeSortKeyTypeCommand = new DelegateCommand<string>(_sidePanelModel.ChangeSortKeyType));

        private DelegateCommand<string> _changeSortOrderTypeCommand;
        public DelegateCommand<string> ChangeSortOrderTypeCommand =>
            _changeSortOrderTypeCommand ?? (_changeSortOrderTypeCommand = new DelegateCommand<string>(_sidePanelModel.ChangeSortOrderType));

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
