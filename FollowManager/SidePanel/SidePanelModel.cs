using System;
using FollowManager.Account;
using FollowManager.FilterAndSort;

namespace FollowManager.SidePanel
{
    public class SidePanelModel
    {
        // プロパティ

        /// <summary>
        /// 現在のフィルタとソートの設定
        /// </summary>
        public FilterAndSortOption FilterAndSortOption { get; } = new FilterAndSortOption();

        // パブリック関数

        /// <summary>
        /// フィルタを変更します。
        /// </summary>
        /// <param name="filterType">変更後のフィルタ</param>
        public void ChangeFilterType(string filterType)
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

        /// <summary>
        /// ソートキ-を変更します。
        /// </summary>
        /// <param name="sortKeyType">変更後のソートキー</param>
        public void ChangeSortKeyType(string sortKeyType)
        {
            switch ((SortKeyType)Enum.Parse(typeof(SortKeyType), sortKeyType))
            {
                case SortKeyType.LastTweetDay:
                    {
                        FilterAndSortOption.SortKeyType = SortKeyType.LastTweetDay;
                        break;
                    }
                case SortKeyType.FollowDay:
                    {
                        FilterAndSortOption.SortKeyType = SortKeyType.FollowDay;
                        break;
                    }
                case SortKeyType.TweetsPerDay:
                    {
                        FilterAndSortOption.SortKeyType = SortKeyType.TweetsPerDay;
                        break;
                    }
            }
        }

        /// <summary>
        /// ソートの順番を変更します。
        /// </summary>
        /// <param name="sortOrderType">変更後のソートの順番</param>
        public void ChangeSortOrderType(string sortOrderType)
        {
            switch ((SortOrderType)Enum.Parse(typeof(SortOrderType), sortOrderType))
            {
                case SortOrderType.Ascending:
                    {
                        FilterAndSortOption.SortOrderType = SortOrderType.Ascending;
                        break;
                    }
                case SortOrderType.Descending:
                    {
                        FilterAndSortOption.SortOrderType = SortOrderType.Descending;
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
