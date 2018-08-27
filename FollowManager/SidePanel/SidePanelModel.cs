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
        public FilterAndSortOption FilterAndSortOption { get; } = new FilterAndSortOption();

        // パブリック関数
        public void ChangeFilter(string filterType)
        {
            switch ((FilterType)Enum.Parse(typeof(FilterType), filterType))
            {
                case FilterType.OneWay:
                    {
                        FilterAndSortOption.FilterType = FilterType.OneWay;
                        break;
                    }
                case FilterType.Fan:
                    {
                        FilterAndSortOption.FilterType = FilterType.Fan;
                        break;
                    }
                case FilterType.Mutual:
                    {
                        FilterAndSortOption.FilterType = FilterType.Mutual;
                        break;
                    }
                case FilterType.Inactive:
                    {
                        FilterAndSortOption.FilterType = FilterType.Inactive;
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
