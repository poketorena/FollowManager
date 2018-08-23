using CoreTweet;
using FollowManager.Account;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using FollowManager.FilterAndSort;
using FollowManager.Service;

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
                await _cardPanelModel.ScheduleBlockAndBlockRelease(x.User);
            }));

        // インタラクションリクエスト

        // プライベート変数
        private IDisposable _filterAndSort;

        // DI注入される変数
        private readonly AccountManager _accountManager;
        private readonly CardPanelModel _cardPanelModel;
        private readonly LoggingService _loggingService;

        // コンストラクタ
        public CardPanelViewModel(AccountManager accountManager, CardPanelModel cardPanelModel, LoggingService loggingService)
        {
            _accountManager = accountManager;
            _cardPanelModel = cardPanelModel;
            _loggingService = loggingService;

            _filterAndSort = _accountManager
                .Current
                .Follows
                .PropertyChangedAsObservable()
                .Subscribe(_ =>
                {
                    UserDatas = new ReactiveCollection<UserData>(_accountManager.Current.Followers.Take(20).ToObservable());
                }
                );

            UserDatas = new ReactiveCollection<UserData>(_accountManager.Current.Followers.Take(20).ToObservable());

            _accountManager
                .Current
                .FilterAndSortOption
                .PropertyChangedAsObservable()
                .Subscribe(x =>
                {
                    if (x.PropertyName == nameof(FilterType))
                    {
                        // 購読を解除
                        _filterAndSort.Dispose();

                        var filterType = _accountManager.Current.FilterAndSortOption.FilterType;

                        switch (filterType)
                        {
                            case FilterType.OneWay:
                                {
                                    var followsObservable = _accountManager
                                    .Current
                                    .Follows
                                    .PropertyChangedAsObservable();

                                    var followersObservable = _accountManager
                                    .Current
                                    .Followers
                                    .PropertyChangedAsObservable();

                                    _filterAndSort = followsObservable
                                    .Zip(followersObservable, (_1, _2) => "unused")
                                    .Subscribe(_ =>
                                    {
                                        UserDatas = new ReactiveCollection<UserData>(
                                            _accountManager
                                            .Current
                                            .Follows
                                            .Except(
                                                _accountManager
                                                .Current
                                                .Follows
                                                .Intersect(
                                                    _accountManager
                                                    .Current
                                                    .Followers,
                                                    new UserDataEqualityComparer()
                                                    ),
                                                new UserDataEqualityComparer()
                                                )
                                                .Select(userData =>
                                                {
                                                    return new UserData
                                                    {
                                                        User = userData.User,
                                                        FollowType = FollowType.OneWay,
                                                        Favorite = userData.Favorite
                                                    };
                                                })
                                                .ToObservable()
                                            );
                                    });

                                    UserDatas = new ReactiveCollection<UserData>(
                                        _accountManager
                                        .Current
                                        .Follows
                                        .Except(
                                            _accountManager
                                            .Current
                                            .Follows
                                            .Intersect(
                                                _accountManager
                                                .Current
                                                .Followers,
                                                new UserDataEqualityComparer()
                                                ),
                                            new UserDataEqualityComparer()
                                            )
                                            .Select(userData =>
                                            {
                                                return new UserData
                                                {
                                                    User = userData.User,
                                                    FollowType = FollowType.OneWay,
                                                    Favorite = userData.Favorite
                                                };
                                            })
                                            .ToObservable()
                                        );
                                    break;
                                }
                            case FilterType.Fan:
                                {
                                    var followsObservable = _accountManager
                                    .Current
                                    .Follows
                                    .PropertyChangedAsObservable();

                                    var followersObservable = _accountManager
                                    .Current
                                    .Followers
                                    .PropertyChangedAsObservable();

                                    _filterAndSort = followsObservable
                                    .Zip(followersObservable, (_1, _2) => "unused")
                                    .Subscribe(_ =>
                                    {
                                        UserDatas = new ReactiveCollection<UserData>(
                                            _accountManager
                                            .Current
                                            .Followers
                                            .Except(
                                                _accountManager
                                                .Current
                                                .Follows
                                                .Intersect(
                                                    _accountManager
                                                    .Current
                                                    .Followers,
                                                    new UserDataEqualityComparer()
                                                    ),
                                                new UserDataEqualityComparer()
                                                )
                                                .Select(userData =>
                                                {
                                                    return new UserData
                                                    {
                                                        User = userData.User,
                                                        FollowType = FollowType.Fan,
                                                        Favorite = userData.Favorite
                                                    };
                                                })
                                                .ToObservable()
                                            );
                                    });

                                    UserDatas = new ReactiveCollection<UserData>(
                                        _accountManager
                                        .Current
                                        .Followers
                                        .Except(
                                            _accountManager
                                            .Current
                                            .Follows
                                            .Intersect(
                                                _accountManager
                                                .Current
                                                .Followers,
                                                new UserDataEqualityComparer()
                                                ),
                                            new UserDataEqualityComparer()
                                            )
                                            .Select(userData =>
                                            {
                                                return new UserData
                                                {
                                                    User = userData.User,
                                                    FollowType = FollowType.Fan,
                                                    Favorite = userData.Favorite
                                                };
                                            })
                                            .ToObservable()
                                        );
                                    break;
                                }
                            case FilterType.Mutual:
                                {
                                    var followsObservable = _accountManager
                                    .Current
                                    .Follows
                                    .PropertyChangedAsObservable();

                                    var followersObservable = _accountManager
                                    .Current
                                    .Followers
                                    .PropertyChangedAsObservable();

                                    _filterAndSort = followsObservable
                                    .Zip(followersObservable, (_1, _2) => "unused")
                                    .Subscribe(_ =>
                                    {
                                        UserDatas = new ReactiveCollection<UserData>(
                                            _accountManager
                                            .Current
                                            .Follows
                                            .Intersect(
                                                _accountManager
                                                .Current
                                                .Followers,
                                                new UserDataEqualityComparer()
                                                )
                                                .Select(userData =>
                                                {
                                                    return new UserData
                                                    {
                                                        User = userData.User,
                                                        FollowType = FollowType.Mutual,
                                                        Favorite = userData.Favorite
                                                    };
                                                })
                                                .ToObservable()
                                            );
                                    });

                                    UserDatas = new ReactiveCollection<UserData>(
                                        _accountManager
                                        .Current
                                        .Follows
                                        .Intersect(
                                            _accountManager
                                            .Current
                                            .Followers,
                                            new UserDataEqualityComparer()
                                            )
                                            .Select(userData =>
                                            {
                                                return new UserData
                                                {
                                                    User = userData.User,
                                                    FollowType = FollowType.Mutual,
                                                    Favorite = userData.Favorite
                                                };
                                            })
                                            .ToObservable()
                                        );
                                    break;
                                }
                            case FilterType.Inactive:
                                {
                                    var followsObservable = _accountManager
                                    .Current
                                    .Follows
                                    .PropertyChangedAsObservable();

                                    var followersObservable = _accountManager
                                    .Current
                                    .Followers
                                    .PropertyChangedAsObservable();

                                    _filterAndSort = followsObservable
                                    .Zip(followersObservable, (_1, _2) => "unused")
                                    .Subscribe(_ =>
                                    {
                                        UserDatas = new ReactiveCollection<UserData>(
                                            _accountManager
                                            .Current
                                            .Follows
                                            .Where(
                                                userData =>
                                                {
                                                    IEnumerable<Status> statuses;
                                                    try
                                                    {
                                                        statuses = _accountManager
                                                        .Current
                                                        .Tokens
                                                        .Statuses
                                                        .UserTimeline(
                                                            user_id => userData.User.Id,
                                                            count => 2
                                                            );

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        _loggingService.Logs.Add(userData.User.ScreenName + " : " + e.Message);
                                                        return false;
                                                    }

                                                    var status = statuses.ElementAtOrDefault(1);

                                                    if (status != null)
                                                    {
                                                        return DateTimeOffset.Now.Subtract(statuses.ElementAt(1).CreatedAt).Days >= 30;
                                                    }
                                                    else
                                                    {
                                                        return true;
                                                    }
                                                })
                                                .Select(userData =>
                                                {
                                                    return new UserData
                                                    {
                                                        User = userData.User,
                                                        FollowType = FollowType.NotSet,
                                                        Favorite = userData.Favorite
                                                    };
                                                })
                                                .ToObservable()
                                            );
                                    });

                                    UserDatas = new ReactiveCollection<UserData>(
                                        _accountManager
                                        .Current
                                        .Follows
                                        .Where(
                                            userData =>
                                            {
                                                IEnumerable<Status> statuses;
                                                try
                                                {
                                                    statuses = _accountManager
                                                    .Current
                                                    .Tokens
                                                    .Statuses
                                                    .UserTimeline(
                                                        user_id => userData.User.Id,
                                                        count => 2
                                                        );

                                                }
                                                catch (Exception e)
                                                {
                                                    _loggingService.Logs.Add(userData.User.ScreenName + " : " + e.Message);
                                                    return true;
                                                }

                                                var status = statuses.ElementAtOrDefault(1);

                                                if (status != null)
                                                {
                                                    return DateTimeOffset.Now.Subtract(statuses.ElementAt(1).CreatedAt).Days >= 30;
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            })
                                            .Select(userData =>
                                            {
                                                return new UserData
                                                {
                                                    User = userData.User,
                                                    FollowType = FollowType.NotSet,
                                                    Favorite = userData.Favorite
                                                };
                                            })
                                            .ToObservable()
                                        );
                                    break;
                                }
                        }
                    }
                });
        }

        // デストラクタ
        ~CardPanelViewModel()
        {
            _filterAndSort.Dispose();
        }

        // プライベート関数
    }
}
