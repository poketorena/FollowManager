using FollowManager.Account;
using FollowManager.EventAggregator;
using FollowManager.FilterAndSort;
using FollowManager.MultiBinding.CommandAndConverterParameter;
using Prism.Events;

namespace FollowManager.SidePanel
{
    public class SidePanelModel
    {
        // パブリックプロパティ

        /// <summary>
        /// 現在のフィルタとソートの設定
        /// </summary>
        public FilterAndSortOption FilterAndSortOption { get; } = new FilterAndSortOption();

        // パブリックメソッド

        /// <summary>
        /// フィルタを変更します。
        /// </summary>
        /// <param name="filterRequest">タブのデータとフィルタの種類</param>
        public void ChangeFilterType(FilterRequest filterRequest)
        {
            switch (filterRequest.FilterType)
            {
                case FilterType.OneWay:
                    {
                        FilterAndSortOption.FilterType = FilterType.OneWay;

                        var filterChangedEventArgs = new FilterChangedEventArgs
                        {
                            TabData = filterRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<FilterChangedEvent>().Publish(filterChangedEventArgs);

                        break;
                    }
                case FilterType.Fan:
                    {
                        FilterAndSortOption.FilterType = FilterType.Fan;

                        var filterChangedEventArgs = new FilterChangedEventArgs
                        {
                            TabData = filterRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<FilterChangedEvent>().Publish(filterChangedEventArgs);

                        break;
                    }
                case FilterType.Mutual:
                    {
                        FilterAndSortOption.FilterType = FilterType.Mutual;

                        var filterChangedEventArgs = new FilterChangedEventArgs
                        {
                            TabData = filterRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<FilterChangedEvent>().Publish(filterChangedEventArgs);

                        break;
                    }
                case FilterType.Inactive:
                    {
                        FilterAndSortOption.FilterType = FilterType.Inactive;

                        var filterChangedEventArgs = new FilterChangedEventArgs
                        {
                            TabData = filterRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<FilterChangedEvent>().Publish(filterChangedEventArgs);

                        break;
                    }
            }
        }

        /// <summary>
        /// ソートキ-を変更します。
        /// </summary>
        /// <param name="sortKeyRequest">タブのデータとソートキーの種類</param>
        public void ChangeSortKeyType(SortKeyRequest sortKeyRequest)
        {
            switch (sortKeyRequest.SortKeyType)
            {
                case SortKeyType.LastTweetDay:
                    {
                        FilterAndSortOption.SortKeyType = SortKeyType.LastTweetDay;

                        var sortKeyChangedEventArgs = new SortKeyChangedEventArgs
                        {
                            TabData = sortKeyRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<SortKeyChangedEvent>().Publish(sortKeyChangedEventArgs);

                        break;
                    }
                case SortKeyType.FollowDay:
                    {
                        FilterAndSortOption.SortKeyType = SortKeyType.FollowDay;

                        var sortKeyChangedEventArgs = new SortKeyChangedEventArgs
                        {
                            TabData = sortKeyRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<SortKeyChangedEvent>().Publish(sortKeyChangedEventArgs);

                        break;
                    }
                case SortKeyType.TweetsPerDay:
                    {
                        FilterAndSortOption.SortKeyType = SortKeyType.TweetsPerDay;

                        var sortKeyChangedEventArgs = new SortKeyChangedEventArgs
                        {
                            TabData = sortKeyRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<SortKeyChangedEvent>().Publish(sortKeyChangedEventArgs);

                        break;
                    }
            }
        }

        /// <summary>
        /// ソート順を変更します。
        /// </summary>
        /// <param name="sortOrderRequest">タブのデータとソート順の種類</param>
        public void ChangeSortOrderType(SortOrderRequest sortOrderRequest)
        {
            switch (sortOrderRequest.SortOrderType)
            {
                case SortOrderType.Ascending:
                    {
                        FilterAndSortOption.SortOrderType = SortOrderType.Ascending;

                        var sortOrderChangedEventArgs = new SortOrderChangedEventArgs
                        {
                            TabData = sortOrderRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<SortOrderChangedEvent>().Publish(sortOrderChangedEventArgs);

                        break;
                    }
                case SortOrderType.Descending:
                    {
                        FilterAndSortOption.SortOrderType = SortOrderType.Descending;

                        var sortOrderChangedEventArgs = new SortOrderChangedEventArgs
                        {
                            TabData = sortOrderRequest.TabData,
                            FilterAndSortOption = FilterAndSortOption
                        };

                        _eventAggregator.GetEvent<SortOrderChangedEvent>().Publish(sortOrderChangedEventArgs);

                        break;
                    }
            }
        }

        // DI注入されるフィールド

        private readonly IEventAggregator _eventAggregator;

        private readonly AccountManager _accountManager;

        // コンストラクタ

        public SidePanelModel(IEventAggregator eventAggregator, AccountManager accountManager)
        {
            _eventAggregator = eventAggregator;
            _accountManager = accountManager;
        }
    }
}
