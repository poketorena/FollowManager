using System;
using FollowManager.Account;
using FollowManager.FilterAndSort;
using FollowManager.MultiBinding.CommandAndConverterParameter;

namespace FollowManager.SidePanel
{
    public class SidePanelModel
    {
        // パブリックプロパティ

        /// <summary>
        /// 現在のフィルタとソートの設定
        /// </summary>
        public FilterAndSortOption FilterAndSortOption { get; } = new FilterAndSortOption();

        // パブリック関数

        /// <summary>
        /// フィルタを変更します。
        /// </summary>
        /// <param name="filterRequest">フィルタを適応するタブのデータとフィルタタイプを指定するためのオブジェクト</param>
        public void ChangeFilterType(FilterRequest filterRequest)
        {
            var filterType = filterRequest.FilterType;
            var tabId = filterRequest.TabData.TabId;

            // ここまでできたのでここからやる（Tokenを使ってAPI呼び出したりする）

            switch ((FilterType)Enum.Parse(typeof(FilterType), filterType))
            {
                case FilterType.OneWay:
                    {
                        // ここでtabIdを使ってCardPanelModelにメッセージを飛ばす！！！
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
        /// ソート順を変更します。
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

        // DI注入される変数

        private readonly AccountManager _accountManager;

        // コンストラクタ

        public SidePanelModel(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }
    }
}
