using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreTweet;
using FollowManager.Account;
using FollowManager.EventAggregator;
using FollowManager.FilterAndSort;
using FollowManager.MultiBinding.MultiParameter;
using FollowManager.Service;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings.Extensions;

namespace FollowManager.CardPanel
{
    public class CardPanelModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのId
        /// </summary>
        public string TabId { get; set; }

        // パブリック関数

        /// <summary>
        /// Twitterのプロフィールページを規定のブラウザで開きます。
        /// </summary>
        /// <param name="screenName">スクリーンネーム</param>
        public void OpenProfile(string screenName)
        {
            var url = "https://twitter.com/" + screenName;

            try
            {
                Process.Start(url);
                _loggingService.Logs.Add($"ブラウザで@{screenName}のプロフィールページを開きました。");
                Debug.WriteLine($"@{screenName}のプロフィールページを開きました。");
            }
            catch (Exception)
            {
                _loggingService.Logs.Add($"ブラウザで@{screenName}のプロフィールページを開くことに失敗しました。");
                Debug.WriteLine($"@{screenName}のプロフィールページを開くことに失敗しました。");
            }
        }

        /// <summary>
        /// 指定したユーザーをブロックして、3秒後にブロック解除します。
        /// </summary>
        /// <param name="blockAndBlockReleaseRequest">タブのデータとブロックしてブロック解除するユーザーデータ</param>
        /// <returns></returns>
        public async Task BlockAndBlockReleaseAsync(BlockAndBlockReleaseRequest blockAndBlockReleaseRequest)
        {
            var userId = blockAndBlockReleaseRequest.TabData.Tokens.UserId;
            var targetId = blockAndBlockReleaseRequest.UserData.User.Id;
            var targetScreenName = blockAndBlockReleaseRequest.UserData.User.Name;

            try
            {
                await _accountManager
                    .Accounts[userId]
                    .Tokens
                    .Blocks
                    .CreateAsync(user_id => targetId)
                    .ConfigureAwait(false);

                var message = $"{targetScreenName}をブロックしました。";
                _loggingService.Logs.Add(message);
                Debug.WriteLine(message);
            }
            catch (KeyNotFoundException)
            {
                var errorMessage = $"{targetScreenName}のブロックに失敗しました。アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
            }
            catch (Exception)
            {
                var errorMessage = $"{targetScreenName}のブロックに失敗しました。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
            }

            await Task.Delay(3000).ConfigureAwait(false);

            try
            {
                await _accountManager
                    .Accounts[userId]
                    .Tokens
                    .Blocks
                    .DestroyAsync(user_id => targetId)
                    .ConfigureAwait(false);

                var message = $"{targetScreenName}のブロックを解除しました。";
                _loggingService.Logs.Add(message);
                Debug.WriteLine(message);
            }
            catch (KeyNotFoundException)
            {
                var errorMessage = $"{targetScreenName}のブロックの解除に失敗しました。アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
            }
            catch (Exception)
            {
                var errorMessage = $"{targetScreenName}のブロックの解除に失敗しました。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
            }
        }

        // イベント

        /// <summary>
        /// ロード完了時に発生するイベント
        /// </summary>
        public event Action<IEnumerable<UserData>> LoadCompleted;

        // プライベートプロパティ

        /// <summary>
        /// IDisposableのコレクション
        /// </summary>
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        /// <summary>
        /// 現在表示中のリスト
        /// </summary>
        private IEnumerable<UserData> _current;

        /// <summary>
        /// 片思いのユーザーのコレクション
        /// </summary>
        private IEnumerable<UserData> _oneWay;

        /// <summary>
        /// ファンのユーザーのコレクション
        /// </summary>
        private IEnumerable<UserData> _fan;

        /// <summary>
        /// 和集合のユーザーのコレクション
        /// </summary>
        private IEnumerable<UserData> _union;

        /// <summary>
        /// 相互フォローのユーザーのコレクション
        /// </summary>
        private IEnumerable<UserData> _mutual;

        /// <summary>
        /// 30日間ツイートしていないユーザーのコレクション
        /// </summary>
        private IEnumerable<UserData> _inactive;

        /// <summary>
        /// フォローしているユーザーのコレクション
        /// </summary>
        private IEnumerable<UserData> _follows;

        /// <summary>
        /// フォローされているユーザーのコレクション
        /// </summary>
        private IEnumerable<UserData> _followers;

        // DI注入される変数

        private readonly IEventAggregator _eventAggregator;

        private readonly AccountManager _accountManager;

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public CardPanelModel(IEventAggregator eventAggregator, AccountManager accountManager, LoggingService loggingService)
        {
            // DI
            _eventAggregator = eventAggregator;
            _accountManager = accountManager;
            _loggingService = loggingService;

            // フィルタの変更を購読してユーザーのリストを読み込む（同じタブからの要求のみ処理する）
            _eventAggregator
                .GetEvent<FilterChangedEvent>()
                .Subscribe(LoadFilteredAndSortedCollection, ThreadOption.PublisherThread, false, filterChangedEventArgs => filterChangedEventArgs.TabData.TabId == TabId)
                .AddTo(Disposables);

            // ソートキーの変更を購読してユーザーのリストを読み込む（同じタブからの要求のみ処理する）
            _eventAggregator
                .GetEvent<SortKeyChangedEvent>()
                .Subscribe(LoadSortedCollection, ThreadOption.PublisherThread, false, sortKeyChangedEventArgs => sortKeyChangedEventArgs.TabData.TabId == TabId)
                .AddTo(Disposables);

            // ソート順の変更を購読してユーザーのリストを読み込む（同じタブからの要求のみ処理する）
            _eventAggregator
                .GetEvent<SortOrderChangedEvent>()
                .Subscribe(LoadSortedCollection, ThreadOption.PublisherThread, false, sortOrderChangedEventArgs => sortOrderChangedEventArgs.TabData.TabId == TabId)
                .AddTo(Disposables);

            // タブが削除されたらリソースを開放する
            _eventAggregator
                .GetEvent<TabRemovedEvent>()
                .Subscribe(_ => Disposables.Dispose(), ThreadOption.PublisherThread, false, tabRemovedEventArgs => tabRemovedEventArgs.TabId == TabId)
                .AddTo(Disposables);
        }

        // プライベート関数

        /// <summary>
        /// フィルタとソートを適応したユーザーのリストを読み込み、完了後にLoadCompletedイベントを発生させます。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        private void LoadFilteredAndSortedCollection(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            switch (sidePanelChangedEventArgs.FilterAndSortOption.FilterType)
            {
                case FilterType.OneWay:
                    {
                        _current = GetOneWayListOrDefault(sidePanelChangedEventArgs) ?? new List<UserData>();
                        SortCurrentList(sidePanelChangedEventArgs);
                        LoadCompleted?.Invoke(_current);
                        break;
                    }
                case FilterType.Fan:
                    {
                        _current = GetFanListOrDefault(sidePanelChangedEventArgs) ?? new List<UserData>();
                        SortCurrentList(sidePanelChangedEventArgs);
                        LoadCompleted?.Invoke(_current);
                        break;
                    }
                case FilterType.Mutual:
                    {
                        _current = GetMutualListOrDefault(sidePanelChangedEventArgs) ?? new List<UserData>();
                        SortCurrentList(sidePanelChangedEventArgs);
                        LoadCompleted?.Invoke(_current);
                        break;
                    }
                case FilterType.Inactive:
                    {
                        _current = GetInactiveListOrDefault(sidePanelChangedEventArgs) ?? new List<UserData>();
                        SortCurrentList(sidePanelChangedEventArgs);
                        LoadCompleted?.Invoke(_current);
                        break;
                    }
            }
        }

        /// <summary>
        /// ソートを適応したユーザーのリストを読み込み、完了後にLoadCompletedイベントを発生させます。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        private void LoadSortedCollection(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_current != null)
            {
                SortCurrentList(sidePanelChangedEventArgs);
                LoadCompleted?.Invoke(_current);
            }
            else
            {
                LoadFilteredAndSortedCollection(sidePanelChangedEventArgs);
            }
        }

        /// <summary>
        /// 現在表示中のリストをソートします。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        private async void SortCurrentList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            var filterType = sidePanelChangedEventArgs.FilterAndSortOption.FilterType;
            var sortKeyType = sidePanelChangedEventArgs.FilterAndSortOption.SortKeyType;
            var sortOrderType = sidePanelChangedEventArgs.FilterAndSortOption.SortOrderType;

            switch (sortKeyType)
            {
                case SortKeyType.LastTweetDay:
                    {
                        var userTweets = GetUserTweetsOrDefault(sidePanelChangedEventArgs);

                        if (userTweets == null)
                        {
                            const string errorMessage = "ソートに失敗しました。アカウントが追加されていません。";
                            _loggingService.Logs.Add(errorMessage);
                            Debug.WriteLine(errorMessage);
                            break;
                        }

                        if (sortOrderType == SortOrderType.Ascending)
                        {
                            // HACK: 非同期処理は要調整
                            _current = _current.OrderBy(userData =>
                            {
                                var result = userTweets
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
                            });
                        }
                        else
                        {
                            _current = _current.OrderByDescending(userData =>
                            {
                                var result = userTweets
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
                            });
                        }
                        break;
                    }
                case SortKeyType.FollowDay:
                    {
                        if (sortOrderType == SortOrderType.Ascending)
                        {
                            switch (filterType)
                            {
                                case FilterType.OneWay:
                                    {
                                        var userDatas = GetOneWayListOrDefault(sidePanelChangedEventArgs)?.Reverse();
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        var userDatas = GetFanListOrDefault(sidePanelChangedEventArgs)?.Reverse();
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        var userDatas = GetMutualListOrDefault(sidePanelChangedEventArgs)?.Reverse();
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        var userDatas = GetInactiveListOrDefault(sidePanelChangedEventArgs)?.Reverse();
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (filterType)
                            {
                                case FilterType.OneWay:
                                    {
                                        var userDatas = GetOneWayListOrDefault(sidePanelChangedEventArgs);
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        var userDatas = GetFanListOrDefault(sidePanelChangedEventArgs);
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        var userDatas = GetMutualListOrDefault(sidePanelChangedEventArgs);
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        var userDatas = GetInactiveListOrDefault(sidePanelChangedEventArgs);
                                        CheckTemporaryUserDatasAndUpdateCurrentUserDataList(userDatas);
                                        break;
                                    }
                            }
                        }
                        break;

                        /// <summary>
                        /// 一時的なユーザーデータをチェックし、現在表示中のリストを更新します。
                        /// </summary>
                        /// <param name="userDatas">一時的なユーザーデータのコレクション</param>
                        void CheckTemporaryUserDatasAndUpdateCurrentUserDataList(IEnumerable<UserData> userDatas)
                        {
                            if (userDatas != null)
                            {
                                _current = userDatas;
                            }
                            else
                            {
                                const string errorMessage = "ソートに失敗しました。アカウントが追加されていません。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
                        }
                    }
                case SortKeyType.TweetsPerDay:
                    {
                        var userTweets = GetUserTweetsOrDefault(sidePanelChangedEventArgs);

                        if (userTweets == null)
                        {
                            const string errorMessage = "ソートに失敗しました。アカウントが追加されていません。";
                            _loggingService.Logs.Add(errorMessage);
                            Debug.WriteLine(errorMessage);
                            break;
                        }

                        if (sortOrderType == SortOrderType.Ascending)
                        {
                            // HACK: 非同期処理は要調整
                            _current = _current.OrderBy(userData =>
                            {
                                var result = userTweets
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
                            });
                        }
                        else
                        {
                            // HACK: 非同期処理は要調整
                            _current = _current.OrderByDescending(userData =>
                            {
                                var result = userTweets
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
                            });
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 片思いのユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>片思いのユーザーのリスト</returns>
        private IEnumerable<UserData> GetOneWayListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_oneWay != null)
            {
                return _oneWay;
            }
            else
            {
                _oneWay = CreateOneWayListOrDefault(sidePanelChangedEventArgs);
                return _oneWay;
            }
        }

        /// <summary>
        /// 片思いのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>片思いのユーザーのリスト</returns>
        private IEnumerable<UserData> CreateOneWayListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return _accountManager
                .Accounts[sidePanelChangedEventArgs.TabData.Tokens.UserId]
                .Follows
                .Except(
                    GetMutualListOrDefault(sidePanelChangedEventArgs),
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
                    });
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "OneWayの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (KeyNotFoundException)
            {
                const string errorMessage = "フィルタに失敗しました。アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// ファンのユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>ファンのユーザーのリスト</returns>
        private IEnumerable<UserData> GetFanListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_fan != null)
            {
                return _fan;
            }
            else
            {
                _fan = CreateFanListOrDefault(sidePanelChangedEventArgs);
                return _fan;
            }
        }

        /// <summary>
        /// ファンのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>ファンのユーザーのリスト</returns>
        private IEnumerable<UserData> CreateFanListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return _accountManager
                    .Accounts[sidePanelChangedEventArgs.TabData.Tokens.UserId]
                    .Followers
                    .Except(
                    GetMutualListOrDefault(sidePanelChangedEventArgs),
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
                    });
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Fanの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (KeyNotFoundException)
            {
                const string errorMessage = "フィルタに失敗しました。アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// 和集合のユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>和集合のユーザーのリスト</returns>
        private IEnumerable<UserData> GetUnionListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_union != null)
            {
                return _union;
            }
            else
            {
                _union = CreateUnionListOrDefault(sidePanelChangedEventArgs);
                return _union;
            }
        }

        /// <summary>
        /// 和集合のユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>和集合のユーザーのリスト</returns>
        private IEnumerable<UserData> CreateUnionListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return GetOneWayListOrDefault(sidePanelChangedEventArgs)
                    .Union(GetMutualListOrDefault(sidePanelChangedEventArgs), new UserDataEqualityComparer())
                    .Union(GetFanListOrDefault(sidePanelChangedEventArgs), new UserDataEqualityComparer());
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Unionの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// 相互フォローのユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>相互フォローのユーザーのリスト</returns>
        private IEnumerable<UserData> GetMutualListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_mutual != null)
            {
                return _mutual;
            }
            else
            {
                _mutual = CreateMutualListOrDefault(sidePanelChangedEventArgs);
                return _mutual;
            }
        }

        /// <summary>
        /// 相互フォローのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>相互フォローのユーザーのリスト</returns>
        private IEnumerable<UserData> CreateMutualListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return _accountManager
                    .Accounts[sidePanelChangedEventArgs.TabData.Tokens.UserId]
                    .Follows
                    .Intersect(
                        _accountManager
                        .Accounts[sidePanelChangedEventArgs.TabData.Tokens.UserId]
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
                        });
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Mutualの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (KeyNotFoundException)
            {
                const string errorMessage = "フィルタに失敗しました。アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// 30日間ツイートしていないユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>30日間ツイートしていないユーザーのリスト</returns>
        private IEnumerable<UserData> GetInactiveListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_inactive != null)
            {
                return _inactive;
            }
            else
            {
                _inactive = CreateInactiveListOrDefault(sidePanelChangedEventArgs);
                return _inactive;
            }
        }

        /// <summary>
        /// 30日間ツイートしていないユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>30日間ツイートしていないユーザーのリスト</returns>
        private IEnumerable<UserData> CreateInactiveListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            var userTweets = GetUserTweetsOrDefault(sidePanelChangedEventArgs);

            if (userTweets == null)
            {
                const string errorMessage = "フィルタに失敗しました。アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }

            try
            {
                return GetUnionListOrDefault(sidePanelChangedEventArgs)
                    .Where(
                    userData =>
                    {
                        var result = userTweets
                        .TryGetValue((long)userData.User.Id, out var statuses);

                        if (!result)
                        {
                            _loggingService.Logs.Add(userData.User.ScreenName + "のツイートはUserTweets内に存在しません。");
                            Debug.WriteLine(userData.User.ScreenName + "のツイートはUserTweets内に存在しません。");
                            return true;
                        }

                        var status = statuses.ElementAtOrDefault(1);

                        if (status != null)
                        {
                            return DateTimeOffset.Now.Subtract(status.CreatedAt).Days >= 30;
                        }
                        else
                        {
                            return true;
                        }
                    });
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Inactiveの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// フォローしているユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>フォローしているユーザーのリスト</returns>
        private IEnumerable<UserData> GetFollowsListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_follows != null)
            {
                return _follows;
            }
            else
            {
                _follows = CreateFollowsListOrDefault(sidePanelChangedEventArgs);
                return _follows;
            }
        }

        /// <summary>
        /// フォローしているユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>フォローしているユーザーのリスト</returns>
        private IEnumerable<UserData> CreateFollowsListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return GetOneWayListOrDefault(sidePanelChangedEventArgs)
                    .Union(GetMutualListOrDefault(sidePanelChangedEventArgs), new UserDataEqualityComparer());
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Followsの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// フォローされているユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>フォローされているユーザーのリスト</returns>
        private IEnumerable<UserData> GetFollowersListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_followers != null)
            {
                return _followers;
            }
            else
            {
                _followers = CreateFollowersListOrDefault(sidePanelChangedEventArgs);
                return _followers;
            }
        }

        /// <summary>
        /// フォローされているユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>フォローされているユーザーのリスト</returns>
        private IEnumerable<UserData> CreateFollowersListOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return GetMutualListOrDefault(sidePanelChangedEventArgs)
                    .Union(GetFanListOrDefault(sidePanelChangedEventArgs), new UserDataEqualityComparer());
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Followersの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// ツイートのリストのディクショナリーを返します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>ツイートのリストのディクショナリー</returns>
        private Dictionary<long, List<Status>> GetUserTweetsOrDefault(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return _accountManager
                    .Accounts[sidePanelChangedEventArgs.TabData.Tokens.UserId]
                    .UserTweets;
            }
            catch (KeyNotFoundException)
            {
                const string errorMessage = "ツイートのリストのディクショナリーの取得に失敗しました。アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                _current = new List<UserData>();
                return null;
            }
        }
    }
}
