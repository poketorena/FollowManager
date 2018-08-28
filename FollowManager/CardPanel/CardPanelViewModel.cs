using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FollowManager.Account;
using FollowManager.FilterAndSort;
using FollowManager.Service;
using FollowManager.SidePanel;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.CardPanel
{
    public class CardPanelViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// 現在表示しているユーザーデータのコレクション
        /// </summary>
        public ReactiveCollection<UserData> UserDatas
        {
            get { return _userDatas; }
            set { SetProperty(ref _userDatas, value); }
        }

        // デリゲートコマンド

        /// <summary>
        /// Twitterのプロフィールページを規定のブラウザで開くコマンド
        /// </summary>
        public DelegateCommand<string> OpenProfileCommand =>
            _openProfileCommand ?? (_openProfileCommand = new DelegateCommand<string>(_cardPanelModel.OpenProfile));

        /// <summary>
        /// お気に入りを切り替えるコマンド
        /// </summary>
        public DelegateCommand<UserData> FavoriteCommnad =>
            _favoriteCommand ?? (_favoriteCommand = new DelegateCommand<UserData>(x =>
            {
                // お気に入りを反転させる
                UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite = !UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite;
            }));

        /// <summary>
        /// 指定したユーザーをブロックして、3秒後にブロック解除するコマンド
        /// </summary>
        public DelegateCommand<UserData> BlockAndBlockReleaseCommand =>
            _blockAndBlockReleaseCommnad ?? (_blockAndBlockReleaseCommnad = new DelegateCommand<UserData>(async x =>
            {
                UserDatas.ElementAt(UserDatas.IndexOf(x)).FollowType = FollowType.BlockAndBlockRelease;
                await _cardPanelModel.BlockAndBlockReleaseAsync(x.User);
            }));

        // プライベートプロパティ

        /// <summary>
        /// IDisposableのコレクション
        /// </summary>
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        private ReactiveCollection<UserData> _userDatas;

        private DelegateCommand<string> _openProfileCommand;

        private DelegateCommand<UserData> _favoriteCommand;

        private DelegateCommand<UserData> _blockAndBlockReleaseCommnad;

        // DI注入される変数

        private readonly AccountManager _accountManager;

        private readonly LoggingService _loggingService;

        private readonly CardPanelModel _cardPanelModel;

        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ

        public CardPanelViewModel(AccountManager accountManager, LoggingService loggingService, CardPanelModel cardPanelModel, SidePanelModel sidePanelModel)
        {
            // DI
            _accountManager = accountManager;
            _loggingService = loggingService;
            _cardPanelModel = cardPanelModel;
            _sidePanelModel = sidePanelModel;

            // 起動時のロード
            //UserDatas = new ReactiveCollection<UserData>(_accountManager.Current.Followers.Take(20).ToObservable());

            // ロード完了時に発生するイベント購読して現在表示しているユーザーデータのコレクションを更新する
            Observable.FromEvent<List<UserData>>(
                handler => _cardPanelModel.LoadCompleted += handler,
                handler => _cardPanelModel.LoadCompleted -= handler
                )
                .Subscribe(userData =>
                {
                    UserDatas = (userData ?? new List<UserData>())
                    .ToObservable()
                    .ToReactiveCollection();
                    Sort();
                })
                .AddTo(Disposables);

            // ソートキー、ソート順を購読して現在表示しているユーザーデータのコレクションをソートする
            _sidePanelModel
                .FilterAndSortOption
                .PropertyChangedAsObservable()
                .Where(args => args.PropertyName == nameof(SortKeyType) || args.PropertyName == nameof(SortOrderType))
                .Subscribe(_ => Sort())
                .AddTo(Disposables);
        }

        // デストラクタ

        ~CardPanelViewModel()
        {
            Disposables.Dispose();
        }

        // プライベート関数

        /// <summary>
        /// 現在表示しているユーザーデータのコレクションをソートします。
        /// </summary>
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
                                        UserDatas = _cardPanelModel
                                            .OneWay
                                            .AsEnumerable()
                                            .Reverse()
                                            .ToObservable()
                                            .ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        UserDatas = _cardPanelModel
                                            .Fan.AsEnumerable()
                                            .Reverse()
                                            .ToObservable()
                                            .ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        UserDatas = _cardPanelModel
                                            .Mutual
                                            .AsEnumerable()
                                            .Reverse()
                                            .ToObservable()
                                            .ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        UserDatas = _cardPanelModel
                                            .Inactive
                                            .AsEnumerable()
                                            .Reverse()
                                            .ToObservable()
                                            .ToReactiveCollection();
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
                                        UserDatas = _cardPanelModel
                                            .OneWay
                                            .ToObservable()
                                            .ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        UserDatas = _cardPanelModel
                                            .Fan
                                            .ToObservable()
                                            .ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        UserDatas = _cardPanelModel
                                            .Mutual
                                            .ToObservable()
                                            .ToReactiveCollection();
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        UserDatas = _cardPanelModel
                                            .Inactive
                                            .ToObservable()
                                            .ToReactiveCollection();
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
