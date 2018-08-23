using FollowManager.Account;
using FollowManager.FilterAndSort;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FollowManager.SidePanel
{
    public class SidePanelViewModel : BindableBase
    {
        // プロパティ

        // パブリック関数

        // デリゲートコマンド
        private DelegateCommand<string> _changeFilterCommand;
        public DelegateCommand<string> ChangeFilterCommand =>
            _changeFilterCommand ?? (_changeFilterCommand = new DelegateCommand<string>(ExecuteChangeFilterCommand));

        // インタラクションリクエスト

        // プライベート変数

        // DI注入される変数
        private readonly AccountManager _accountManager;

        // コンストラクタ
        public SidePanelViewModel(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        // デストラクタ

        // プライベート関数
        private void ExecuteChangeFilterCommand(string filterParamer)
        {
            switch (filterParamer)
            {
                case nameof(FilterType.OneWay):
                    {
                        _accountManager.Current.FilterAndSortOption.FilterType = FilterType.OneWay;
                        break;
                    }
                case nameof(FilterType.Fan):
                    {
                        _accountManager.Current.FilterAndSortOption.FilterType = FilterType.Fan;
                        break;
                    }
                case nameof(FilterType.Mutual):
                    {
                        _accountManager.Current.FilterAndSortOption.FilterType = FilterType.Mutual;
                        break;
                    }
                case nameof(FilterType.Inactive):
                    {
                        _accountManager.Current.FilterAndSortOption.FilterType = FilterType.Inactive;
                        break;
                    }
                default:
                    break;
            }

        }
    }
}
