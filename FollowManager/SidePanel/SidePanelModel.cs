using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FollowManager.Account;
using FollowManager.FilterAndSort;

namespace FollowManager.SidePanel
{
    public class SidePanelModel
    {
        // プロパティ

        // パブリック関数
        public void ExecuteChangeFilterCommand(string filterParamer)
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
            }
        }

        // プライベート変数

        // DI注入される変数
        private readonly AccountManager _accountManager;
        
        // コンストラクタ
        public SidePanelModel(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }
        // デストラクタ

        // プライベート関数
    }
}
