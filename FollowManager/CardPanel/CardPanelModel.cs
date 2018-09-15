using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
                    .Accounts
                    .Single(account => account.User.Id == userId)
                    .Tokens
                    .Blocks
                    .CreateAsync(user_id => targetId)
                    .ConfigureAwait(false);

                var message = $"{targetScreenName}をブロックしました。";
                _loggingService.Logs.Add(message);
                Debug.WriteLine(message);
            }
            catch (InvalidOperationException)
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
                    .Accounts
                    .Single(account => account.User.Id == userId)
                    .Tokens
                    .Blocks
                    .DestroyAsync(user_id => targetId)
                    .ConfigureAwait(false);

                var message = $"{targetScreenName}のブロックを解除しました。";
                _loggingService.Logs.Add(message);
                Debug.WriteLine(message);
            }
            catch (InvalidOperationException)
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
                .Subscribe(LoadFilteredAndSortedCollection, ThreadOption.PublisherThread, false, filter => filter.TabData.TabId == TabId)
                .AddTo(Disposables);

            // ソートキーの変更を購読してユーザーのリストを読み込む（同じタブからの要求のみ処理する）
            _eventAggregator
                .GetEvent<SortKeyChangedEvent>()
                .Subscribe(LoadSortedCollection, ThreadOption.PublisherThread, false, filter => filter.TabData.TabId == TabId)
                .AddTo(Disposables);

            // ソート順の変更を購読してユーザーのリストを読み込む（同じタブからの要求のみ処理する）
            _eventAggregator
                .GetEvent<SortOrderChangedEvent>()
                .Subscribe(LoadSortedCollection, ThreadOption.PublisherThread, false, filter => filter.TabData.TabId == TabId)
                .AddTo(Disposables);
        }

        // デストラクタ

        ~CardPanelModel()
        {
            Disposables.Dispose();
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
                        _current = GetOneWayList(sidePanelChangedEventArgs) ?? new List<UserData>();
                        SortCurrentList(sidePanelChangedEventArgs);
                        LoadCompleted?.Invoke(_current);
                        break;
                    }
                case FilterType.Fan:
                    {
                        _current = GetFanList(sidePanelChangedEventArgs) ?? new List<UserData>();
                        SortCurrentList(sidePanelChangedEventArgs);
                        LoadCompleted?.Invoke(_current);
                        break;
                    }
                case FilterType.Mutual:
                    {
                        _current = GetMutualList(sidePanelChangedEventArgs) ?? new List<UserData>();
                        SortCurrentList(sidePanelChangedEventArgs);
                        LoadCompleted?.Invoke(_current);
                        break;
                    }
                case FilterType.Inactive:
                    {
                        _current = GetInactiveList(sidePanelChangedEventArgs) ?? new List<UserData>();
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
                        if (sortOrderType == SortOrderType.Ascending)
                        {
                            try
                            {
                                // HACK: 非同期処理は要調整
                                _current = _current.OrderBy(userData =>
                                {
                                    var result = _accountManager
                                .Accounts
                                .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
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
                                });
                            }
                            catch (InvalidOperationException)
                            {
                                const string errorMessage = "アカウントが追加されていません。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
                            catch (ArgumentNullException)
                            {
                                const string errorMessage = "ソートに失敗しました。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
                        }
                        else
                        {
                            try
                            {
                                // HACK: 非同期処理は要調整
                                _current = _current.OrderByDescending(userData =>
                                {
                                    var result = _accountManager
                                    .Accounts
                                    .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
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
                                });
                            }
                            catch (InvalidOperationException)
                            {
                                const string errorMessage = "アカウントが追加されていません。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
                            catch (ArgumentNullException)
                            {
                                const string errorMessage = "ソートに失敗しました。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
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
                                        _current = GetOneWayList(sidePanelChangedEventArgs)?.Reverse() ?? new List<UserData>();
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        _current = GetFanList(sidePanelChangedEventArgs)?.Reverse() ?? new List<UserData>();
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        _current = GetMutualList(sidePanelChangedEventArgs)?.Reverse() ?? new List<UserData>();
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        _current = GetInactiveList(sidePanelChangedEventArgs)?.Reverse() ?? new List<UserData>();
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
                                        _current = GetOneWayList(sidePanelChangedEventArgs) ?? new List<UserData>();
                                        break;
                                    }
                                case FilterType.Fan:
                                    {
                                        _current = GetFanList(sidePanelChangedEventArgs) ?? new List<UserData>();
                                        break;
                                    }
                                case FilterType.Mutual:
                                    {
                                        _current = GetMutualList(sidePanelChangedEventArgs) ?? new List<UserData>();
                                        break;
                                    }
                                case FilterType.Inactive:
                                    {
                                        _current = GetInactiveList(sidePanelChangedEventArgs) ?? new List<UserData>();
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
                            try
                            {
                                // HACK: 非同期処理は要調整
                                _current = _current.OrderBy(userData =>
                                {
                                    var result = _accountManager
                                    .Accounts
                                    .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
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
                                });
                            }
                            catch (InvalidOperationException)
                            {
                                const string errorMessage = "アカウントが追加されていません。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
                            catch (ArgumentNullException)
                            {
                                const string errorMessage = "ソートに失敗しました。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
                        }
                        else
                        {
                            try
                            {
                                // HACK: 非同期処理は要調整
                                _current = _current.OrderByDescending(userData =>
                                {
                                    var result = _accountManager
                                    .Accounts
                                    .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
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
                                });
                            }
                            catch (InvalidOperationException)
                            {
                                const string errorMessage = "アカウントが追加されていません。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
                            catch (ArgumentNullException)
                            {
                                const string errorMessage = "ソートに失敗しました。";
                                _loggingService.Logs.Add(errorMessage);
                                Debug.WriteLine(errorMessage);
                                _current = new List<UserData>();
                            }
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
        private IEnumerable<UserData> GetOneWayList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_oneWay != null)
            {
                return _oneWay;
            }
            else
            {
                _oneWay = CreateOneWayList(sidePanelChangedEventArgs);
                return _oneWay;
            }
        }

        /// <summary>
        /// 片思いのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>片思いのユーザーのリスト</returns>
        private IEnumerable<UserData> CreateOneWayList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return _accountManager
                .Accounts
                .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
                .Follows
                .Except(
                    GetMutualList(sidePanelChangedEventArgs),
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
            catch (InvalidOperationException)
            {
                const string errorMessage = "アカウントが追加されていません。";
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
        private IEnumerable<UserData> GetFanList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_fan != null)
            {
                return _fan;
            }
            else
            {
                _fan = CreateFanList(sidePanelChangedEventArgs);
                return _fan;
            }
        }

        /// <summary>
        /// ファンのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>ファンのユーザーのリスト</returns>
        private IEnumerable<UserData> CreateFanList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return _accountManager
                    .Accounts
                    .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
                    .Followers
                    .Except(
                    GetMutualList(sidePanelChangedEventArgs),
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
            catch (InvalidOperationException)
            {
                const string errorMessage = "アカウントが追加されていません。";
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
        private IEnumerable<UserData> GetUnionList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_union != null)
            {
                return _union;
            }
            else
            {
                _union = CreateUnionList(sidePanelChangedEventArgs);
                return _union;
            }
        }

        /// <summary>
        /// 和集合のユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>和集合のユーザーのリスト</returns>
        private IEnumerable<UserData> CreateUnionList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return GetOneWayList(sidePanelChangedEventArgs)
                    .Union(GetMutualList(sidePanelChangedEventArgs), new UserDataEqualityComparer())
                    .Union(GetFanList(sidePanelChangedEventArgs), new UserDataEqualityComparer());
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
        private IEnumerable<UserData> GetMutualList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_mutual != null)
            {
                return _mutual;
            }
            else
            {
                _mutual = CreateMutualList(sidePanelChangedEventArgs);
                return _mutual;
            }
        }

        /// <summary>
        /// 相互フォローのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>相互フォローのユーザーのリスト</returns>
        private IEnumerable<UserData> CreateMutualList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return _accountManager
                    .Accounts
                    .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
                    .Follows
                    .Intersect(
                        _accountManager
                        .Accounts
                        .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
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
            catch (InvalidOperationException)
            {
                const string errorMessage = "アカウントが追加されていません。";
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
        private IEnumerable<UserData> GetInactiveList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_inactive != null)
            {
                return _inactive;
            }
            else
            {
                _inactive = CreateInactiveList(sidePanelChangedEventArgs);
                return _inactive;
            }
        }

        /// <summary>
        /// 30日間ツイートしていないユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>30日間ツイートしていないユーザーのリスト</returns>
        private IEnumerable<UserData> CreateInactiveList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return GetUnionList(sidePanelChangedEventArgs)
                    .Where(
                    userData =>
                    {
                        var result = _accountManager
                        .Accounts
                        .Single(account => account.Tokens.ScreenName == sidePanelChangedEventArgs.TabData.Tokens.ScreenName)
                        .UserTweets
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
            catch (InvalidOperationException)
            {
                const string errorMessage = "アカウントが追加されていません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// フォローしているユーザーのリストを取得します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>フォローしているユーザーのリスト</returns>
        private IEnumerable<UserData> GetFollowsList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_follows != null)
            {
                return _follows;
            }
            else
            {
                _follows = CreateFollowsList(sidePanelChangedEventArgs);
                return _follows;
            }
        }

        /// <summary>
        /// フォローしているユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>フォローしているユーザーのリスト</returns>
        private IEnumerable<UserData> CreateFollowsList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return GetOneWayList(sidePanelChangedEventArgs)
                    .Union(GetMutualList(sidePanelChangedEventArgs), new UserDataEqualityComparer());
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
        private IEnumerable<UserData> GetFollowersList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            if (_followers != null)
            {
                return _followers;
            }
            else
            {
                _followers = CreateFollowersList(sidePanelChangedEventArgs);
                return _followers;
            }
        }

        /// <summary>
        /// フォローされているユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <param name="sidePanelChangedEventArgs">SidePanelで発生したイベントデータ</param>
        /// <returns>フォローされているユーザーのリスト</returns>
        private IEnumerable<UserData> CreateFollowersList(ISidePanelChangedEventArgs sidePanelChangedEventArgs)
        {
            try
            {
                return GetMutualList(sidePanelChangedEventArgs)
                    .Union(GetFanList(sidePanelChangedEventArgs), new UserDataEqualityComparer());
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Followersの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }
    }
}
