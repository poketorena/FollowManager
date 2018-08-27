using FollowManager.Account;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using FollowManager.Service;
using Prism.Regions;
using System.Diagnostics;
using FollowManager.SidePanel;
using System.Threading.Tasks;
using System.ComponentModel;
using FollowManager.FilterAndSort;

namespace FollowManager.CardPanel
{
    public class CardPanelViewModel : BindableBase
    {
        // プロパティ
        private ReactiveCollection<UserData> _userDatas;
        public ReactiveCollection<UserData> UserDatas
        {
            get { return _userDatas; }
            set { SetProperty(ref _userDatas, value); }
        }

        // パブリック関数

        // デリゲートコマンド
        private DelegateCommand<string> _openProfileCommand;
        public DelegateCommand<string> OpenProfileCommand =>
            _openProfileCommand ?? (_openProfileCommand = new DelegateCommand<string>(_cardPanelModel.OpenProfile));

        private DelegateCommand<UserData> _favoriteCommand;
        public DelegateCommand<UserData> FavoriteCommnad =>
            _favoriteCommand ?? (_favoriteCommand = new DelegateCommand<UserData>(x =>
            {
                // Favoriteを反転させる
                UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite = !UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite;
            }));

        private DelegateCommand<UserData> _blockAndBlockReleaseCommnad;
        public DelegateCommand<UserData> BlockAndBlockReleaseCommand =>
            _blockAndBlockReleaseCommnad ?? (_blockAndBlockReleaseCommnad = new DelegateCommand<UserData>(async x =>
            {
                UserDatas.ElementAt(UserDatas.IndexOf(x)).FollowType = FollowType.BlockAndBlockRelease;
                await _cardPanelModel.BlockAndBlockReleaseAsync(x.User);
            }));

        // インタラクションリクエスト

        // プライベート変数
        private IDisposable _filter;
        private IDisposable _sort;

        // DI注入される変数
        private readonly AccountManager _accountManager;
        private readonly LoggingService _loggingService;
        private readonly CardPanelModel _cardPanelModel;
        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ
        public CardPanelViewModel(AccountManager accountManager, LoggingService loggingService, CardPanelModel cardPanelModel, SidePanelModel sidePanelModel)
        {
            _accountManager = accountManager;
            _loggingService = loggingService;
            _cardPanelModel = cardPanelModel;
            _sidePanelModel = sidePanelModel;

            //UserDatas = new ReactiveCollection<UserData>(_accountManager.Current.Followers.Take(20).ToObservable());

            _filter = Observable.FromEvent<List<UserData>>(
                handler => _cardPanelModel.LoadCompleted += handler,
                handler => _cardPanelModel.LoadCompleted -= handler
                )
                .Subscribe(userData =>
                {
                    UserDatas = (userData ?? new List<UserData>())
                    .ToObservable()
                    .ToReactiveCollection();
                });

            _sort = _sidePanelModel
                .FilterAndSortOption
                .PropertyChangedAsObservable()
                .Where(args => args.PropertyName == nameof(SortKeyType) || args.PropertyName == nameof(SortOrderType))
                .Subscribe(_ => Sort());
        }

        // デストラクタ
        ~CardPanelViewModel()
        {
            _filter.Dispose();
            _sort.Dispose();
        }

        // プライベート関数
        private async void Sort()
        {
            var sortKeyType = _sidePanelModel.FilterAndSortOption.SortKeyType;

            var sortOrderType = _sidePanelModel.FilterAndSortOption.SortOrderType;

            switch (sortKeyType)
            {
                case SortKeyType.LastTweetDay:
                    {
                        if (sortOrderType == SortOrderType.Ascending)
                        {
                            // HACK: 非同期処理は要調整
                            UserDatas = await Task.Run(() => UserDatas.OrderBy(userData =>
                            {
                                var result = _accountManager
                                .Current
                                .UserTweets
                                .TryGetValue((long)userData.User.Id, out var statuses);
                                if (result)
                                {
                                    var status = statuses.ElementAtOrDefault(1);

                                    if (status != null)
                                    {
                                        return status.CreatedAt.UtcDateTime;
                                    }
                                    else
                                    {
                                        // ユーザーのツイート数が少なすぎるため末尾に追加（昇順なのでUTCの最大値を返すと末尾になる）
                                        return DateTime.MaxValue.ToUniversalTime();
                                    }
                                }
                                else
                                {
                                    // UserTweetsからツイートが見つからなかったため末尾に追加（昇順なのでUTCの最大値を返すと末尾になる）
                                    return DateTime.MaxValue.ToUniversalTime();
                                }
                            })
                            .ToObservable()
                            .ToReactiveCollection());
                        }
                        else
                        {
                            // HACK: 非同期処理は要調整
                            UserDatas = await Task.Run(() => UserDatas.OrderByDescending(userData =>
                            {
                                var result = _accountManager
                                .Current
                                .UserTweets
                                .TryGetValue((long)userData.User.Id, out var statuses);
                                if (result)
                                {
                                    var status = statuses.ElementAtOrDefault(1);

                                    if (status != null)
                                    {
                                        return status.CreatedAt.UtcDateTime;
                                    }
                                    else
                                    {
                                        // ユーザーのツイート数が少なすぎるため末尾に追加（降順なのでUTCの最小値を返すと末尾になる）
                                        return DateTime.MinValue.ToUniversalTime();
                                    }
                                }
                                else
                                {
                                    // UserTweetsからツイートが見つからなかったため末尾に追加（降順なのでUTCの最小値を返すと末尾になる）
                                    return DateTime.MinValue.ToUniversalTime();
                                }
                            })
                            .ToObservable()
                            .ToReactiveCollection());
                        }
                        break;
                    }
                case SortKeyType.FollowDay:
                    {
                        if (sortOrderType == SortOrderType.Ascending)
                        {
                            var filterType = _sidePanelModel.FilterAndSortOption.FilterType;
                            switch (filterType)
                            {
                                case FilterType.OneWay:
                                    {
                                        UserDatas = _cardPanelModel.OneWay.AsEnumerable().Reverse().ToObservable().ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        UserDatas = _cardPanelModel.Fan.AsEnumerable().Reverse().ToObservable().ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        UserDatas = _cardPanelModel.Mutual.AsEnumerable().Reverse().ToObservable().ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        UserDatas = _cardPanelModel.Inactive.AsEnumerable().Reverse().ToObservable().ToReactiveCollection();
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            var filterType = _sidePanelModel.FilterAndSortOption.FilterType;
                            switch (filterType)
                            {
                                case FilterType.OneWay:
                                    {
                                        UserDatas = _cardPanelModel.OneWay.ToObservable().ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        UserDatas = _cardPanelModel.Fan.ToObservable().ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        UserDatas = _cardPanelModel.Mutual.ToObservable().ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        UserDatas = _cardPanelModel.Inactive.ToObservable().ToReactiveCollection();
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case SortKeyType.TweetsPerDay:
                    {
                        if (sortOrderType == SortOrderType.Ascending)
                        {
                            // HACK: 非同期処理は要調整
                            UserDatas = await Task.Run(() => UserDatas.OrderBy(userData =>
                            {
                                var result = _accountManager
                                .Current
                                .UserTweets
                                .TryGetValue((long)userData.User.Id, out var statuses);

                                if (result)
                                {
                                    var recentlyStatus = statuses.ElementAtOrDefault(1);
                                    var oldStatus = statuses.LastOrDefault();

                                    if (recentlyStatus != null && oldStatus != null)
                                    {
                                        var days = (recentlyStatus.CreatedAt - oldStatus.CreatedAt).Days + 1;
                                        return statuses.Count / days;
                                    }
                                    else
                                    {
                                        // ユーザーのツイート数が少なすぎるため末尾に追加（昇順なのでint型の最大値を返すと末尾になる）
                                        return int.MaxValue;
                                    }
                                }
                                else
                                {
                                    // UserTweetsからツイートが見つからなかったため末尾に追加（昇順なのでint型の最大値を返すと末尾になる）
                                    return int.MaxValue;
                                }
                            })
                            .ToObservable()
                            .ToReactiveCollection());
                        }
                        else
                        {
                            // HACK: 非同期処理は要調整
                            UserDatas = await Task.Run(() => UserDatas.OrderByDescending(userData =>
                            {
                                var result = _accountManager
                                .Current
                                .UserTweets
                                .TryGetValue((long)userData.User.Id, out var statuses);

                                if (result)
                                {
                                    var recentlyStatus = statuses.ElementAtOrDefault(1);
                                    var oldStatus = statuses.LastOrDefault();

                                    if (recentlyStatus != null && oldStatus != null)
                                    {
                                        var days = (recentlyStatus.CreatedAt - oldStatus.CreatedAt).Days + 1;
                                        return statuses.Count / days;
                                    }
                                    else
                                    {
                                        // ユーザーのツイート数が少なすぎるため末尾に追加（降順なのでint型の最小値を返すと末尾になる）
                                        return int.MinValue;
                                    }
                                }
                                else
                                {
                                    // UserTweetsからツイートが見つからなかったため末尾に追加（降順なのでint型の最小値を返すと末尾になる）
                                    return int.MinValue;
                                }
                            })
                            .ToObservable()
                            .ToReactiveCollection());
                        }
                        break;
                    }
            }
        }
    }
}
