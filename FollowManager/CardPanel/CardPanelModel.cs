using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreTweet;
using FollowManager.Account;
using FollowManager.FilterAndSort;
using FollowManager.Service;
using FollowManager.SidePanel;
using Prism.Mvvm;
using Reactive.Bindings.Extensions;

namespace FollowManager.CardPanel
{
    public class CardPanelModel : BindableBase
    {
        // プロパティ

        /// <summary>
        /// 片思いのユーザーのリスト
        /// </summary>
        public List<UserData> OneWay =>
            _oneWay ?? (_oneWay = CreateOneWayList());

        /// <summary>
        /// ファンのユーザーのリスト
        /// </summary>
        public List<UserData> Fan =>
            _fan ?? (_fan = CreateFanList());

        /// <summary>
        /// 和集合のユーザーのリスト
        /// </summary>
        public List<UserData> Union =>
            _union ?? (_union = CreateUnionList());

        /// <summary>
        /// 相互フォローのユーザーのリスト
        /// </summary>
        public List<UserData> Mutual =>
            _mutual ?? (_mutual = CreateMutualList());

        /// <summary>
        /// 30日間ツイートしていないユーザーのリスト
        /// </summary>
        public List<UserData> Inactive =>
            _inactive ?? (_inactive = CreateInactiveList());

        /// <summary>
        /// フォローしているユーザーのリスト
        /// </summary>
        public List<UserData> Follows =>
            _follows ?? (_follows = CreateFollowsList());

        /// <summary>
        /// フォローされているユーザーのリスト
        /// </summary>
        public List<UserData> Followers =>
            _followers ?? (_followers = CreateFollowersList());

        // パブリック関数

        /// <summary>
        /// Twitterのプロフィールページを規定のブラウザで開きます。
        /// </summary>
        /// <param name="screenName">@hogeの場合はhoge</param>
        public void OpenProfile(string screenName)
        {
            var url = "https://twitter.com/" + screenName;

            try
            {
                System.Diagnostics.Process.Start(url);
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
        /// <param name="user">ブロックしてブロック解除するユーザー</param>
        /// <returns></returns>
        public async Task BlockAndBlockReleaseAsync(User user)
        {
            try
            {
                await _accountManager.Current.Tokens.Blocks.CreateAsync(user_id => user.Id);
                _loggingService.Logs.Add($"{user.Name}をブロックしました。");
                Debug.WriteLine($"{user.Name}をブロックしました。");
            }
            catch (Exception)
            {
                _loggingService.Logs.Add($"{user.Name}のブロックに失敗しました。");
                Debug.WriteLine($"{user.Name}のブロックに失敗しました。");
            }

            await Task.Delay(3000);

            try
            {
                await _accountManager.Current.Tokens.Blocks.DestroyAsync(user_id => user.Id);
                _loggingService.Logs.Add($"{user.Name}のブロックを解除しました。");
                Debug.WriteLine($"{user.Name}のブロックを解除しました。");
            }
            catch (Exception)
            {
                _loggingService.Logs.Add($"{user.Name}のブロックの解除に失敗しました。");
                Debug.WriteLine($"{user.Name}のブロックの解除に失敗しました。");
            }
        }

        // イベント

        /// <summary>
        /// ロード完了時に発生するイベント
        /// </summary>
        public event Action<List<UserData>> LoadCompleted;

        // プライベート変数

        private List<UserData> _oneWay;

        private List<UserData> _fan;

        private List<UserData> _union;

        private List<UserData> _mutual;

        private List<UserData> _inactive;

        private List<UserData> _follows;

        private List<UserData> _followers;

        private readonly IDisposable _filter;

        // DI注入される変数

        private readonly AccountManager _accountManager;

        private readonly LoggingService _loggingService;

        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ

        public CardPanelModel(AccountManager accountManager, LoggingService loggingService, SidePanelModel sidePanelModel)
        {
            // DI
            _accountManager = accountManager;
            _loggingService = loggingService;
            _sidePanelModel = sidePanelModel;

            // フィルタの変更を購読してユーザーのリストを読み込む
            _filter = _sidePanelModel
                .FilterAndSortOption
                .PropertyChangedAsObservable()
                .Where(args => args.PropertyName == nameof(FilterType))
                // HACK: 非同期処理は要調整
                .Subscribe(_ => Task.Run(() => Load()));
        }

        // デストラクタ
        ~CardPanelModel()
        {
            _filter.Dispose();
        }
        // プライベート関数

        /// <summary>
        /// ユーザーのリストを読み込み、完了後にLoadCompletedイベントを発生させます。
        /// </summary>
        private void Load()
        {
            var filterType = _sidePanelModel.FilterAndSortOption.FilterType;

            switch (filterType)
            {
                case FilterType.OneWay:
                    {
                        LoadCompleted?.Invoke(OneWay);
                        break;
                    }
                case FilterType.Fan:
                    {
                        LoadCompleted?.Invoke(Fan);
                        break;
                    }
                case FilterType.Mutual:
                    {
                        LoadCompleted?.Invoke(Mutual);
                        break;
                    }
                case FilterType.Inactive:
                    {
                        LoadCompleted?.Invoke(Inactive);
                        break;
                    }
            }
        }

        /// <summary>
        /// 片思いのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <returns></returns>
        private List<UserData> CreateOneWayList()
        {
            try
            {
                return _accountManager
                .Current
                .Follows
                .Except(
                    Mutual,
                    new UserDataEqualityComparer()
                    )
                    .Select(userData =>
                    {
                        return new UserData
                        (
                            userData.User,
                            FollowType.OneWay,
                            userData.Favorite
                        );
                    })
                    .ToList();
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "OneWayの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// ファンのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <returns></returns>
        private List<UserData> CreateFanList()
        {
            try
            {
                return _accountManager
                .Current
                .Followers
                .Except(
                    Mutual,
                    new UserDataEqualityComparer()
                    )
                    .Select(userData =>
                    {
                        return new UserData
                        (
                            userData.User,
                            FollowType.Fan,
                            userData.Favorite
                        );
                    })
                    .ToList();
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Fanの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// 和集合のユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <returns></returns>
        private List<UserData> CreateUnionList()
        {
            try
            {
                return OneWay
                    .Union(Mutual)
                    .Union(Fan)
                    .ToList();
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Unionの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// 相互フォローのユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <returns></returns>
        private List<UserData> CreateMutualList()
        {
            try
            {
                return _accountManager
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
                            (
                                userData.User,
                                FollowType.Mutual,
                                userData.Favorite
                            );
                        })
                        .ToList();
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Mutualの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// 30日間ツイートしていないユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <returns></returns>
        private List<UserData> CreateInactiveList()
        {
            try
            {
                return Union
                    .Where(
                    userData =>
                    {
                        var result = _accountManager
                        .Current
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
                    })
                    .ToList();
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Inactiveの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// フォローしているユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <returns></returns>
        private List<UserData> CreateFollowsList()
        {
            try
            {
                return OneWay
                    .Union(Mutual)
                    .ToList();
            }
            catch (ArgumentNullException)
            {
                const string errorMessage = "Followsの作成に失敗しました。LINQの引数が null です。";
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// フォローされているユーザーのリストを作成します。例外発生時はnullを返します。
        /// </summary>
        /// <returns></returns>
        private List<UserData> CreateFollowersList()
        {
            try
            {
                return Mutual
                    .Union(Fan)
                    .ToList();
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
