using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using CoreTweet;
using FollowManager.Service;
using MessagePack;
using MessagePack.Resolvers;

namespace FollowManager.Account
{
    /// <summary>
    /// CoreTweetのトークンの保持、TwitterApiまたはローカルからのデータの取得及び保持を行います。
    /// </summary>
    public class Account
    {
        // パブリックプロパティ

        /// <summary>
        /// XAML編集中にAPIを呼び出すのを防ぐフラグ
        /// </summary>
        public static bool IsInDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        /// <summary>
        /// フォローしているユーザーのリスト
        /// </summary>
        public List<UserData> Follows =>
            _follows ?? (_follows = GetFollows());

        /// <summary>
        /// フォローされているユーザーのリスト
        /// </summary>
        public List<UserData> Followers =>
            _followers ?? (_followers = GetFollowers());

        /// <summary>
        /// ツイートのリストのディクショナリー
        /// </summary>
        /// <value>キーはユーザーId</value>
        public Dictionary<long, List<Status>> UserTweets =>
            _userTweets ?? (_userTweets = GetUserTweets());

        /// <summary>
        /// CoreTweetのトークン
        /// </summary>
        public Tokens Tokens { get; set; }

        /// <summary>
        /// 自分のUserデータ
        /// </summary>
        public User User =>
            _user ?? (_user = GetMyUserData());

        // プライベート変数

        private List<UserData> _follows;

        private List<UserData> _followers;

        private Dictionary<long, List<Status>> _userTweets;

        private User _user;

        // DI注入される変数

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public Account(LoggingService loggingService)
        {
            // DI
            _loggingService = loggingService;
        }

        // プライベート関数

        /// <summary>
        /// フォローしているユーザーのリストを取得します。
        /// </summary>
        /// <returns>フォローしているユーザーのリストを返します。例外発生時はnullを返します。</returns>
        private List<UserData> GetFollows()
        {
            var userDatas = new List<UserData>();

            if (!Directory.Exists($@"Data\{Tokens.ScreenName}"))
            {
                if (!TryCreateDataDirectory())
                {
                    return null;
                }
            }

            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Follows.data";

            if (File.Exists($@"Data\{Tokens.ScreenName}\{fileName}"))
            {
                // ローカルにデータが存在する場合はデシリアライズして返す
                return GetFollowsFromLocal();
            }
            else
            {
                // ローカルにデータが存在しない場合はTwitterApiを呼び出して取得する
                return GetFollowsFromTwitterApi();
            }
        }

        /// <summary>
        /// ローカルからフォローしているユーザーのリストを取得します。
        /// </summary>
        /// <returns>フォローしているユーザーのリストを返します。例外発生時はnullを返します。</returns>
        private List<UserData> GetFollowsFromLocal()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Follows.data";

            try
            {
                using (var streamReader = File.OpenText($@"Data\{Tokens.ScreenName}\{fileName}"))
                {
                    var userDatas = new List<UserData>();
                    try
                    {
                        userDatas = LZ4MessagePackSerializer.Deserialize<List<UserData>>(streamReader.BaseStream, TypelessContractlessStandardResolver.Instance);
                    }
                    catch (Exception)
                    {
                        var errorMessage = $"{fileName} を開くことに失敗しました。ファイルが壊れているためデータを再取得します。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return GetFollowsFromTwitterApi();
                    }
                    return userDatas;
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// TwitterApiからフォローしているユーザーのリストを取得します。
        /// </summary>
        /// <returns>フォローしているユーザーのリストを返します。例外発生時はnullを返します。</returns>
        private List<UserData> GetFollowsFromTwitterApi()
        {
            if (!IsInDesignMode)
            {
                var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Follows.data";

                var userDatas = new List<UserData>();

                try
                {
                    for (long cursorTmp = -1; cursorTmp != 0;)
                    {
                        var friends = Tokens.Friends.List(
                            user_id => Tokens.UserId,
                            cursor => cursorTmp,
                            count => 200
                            );

                        userDatas.AddRange(friends.Result.Select(user => new UserData { User = user, FollowType = FollowType.NotSet, Favorite = false }));

                        cursorTmp = friends.NextCursor;
                    }
                }
                catch (Exception error)
                {
                    _loggingService.Logs.Add($"フォロー一覧の取得に失敗しました。{error.Message}");
                    Debug.WriteLine($"フォロー一覧の取得に失敗しました。{error.Message}");
                    return null;
                }

                try
                {
                    using (var streamWriter = File.CreateText($@"Data\{Tokens.ScreenName}\{fileName}"))
                    {
                        try
                        {
                            LZ4MessagePackSerializer.Serialize(streamWriter.BaseStream, userDatas, TypelessContractlessStandardResolver.Instance);
                        }
                        catch (Exception)
                        {
                            var errorMessage = $"{fileName} の保存に失敗しました。";
                            _loggingService.Logs.Add(errorMessage);
                            Debug.WriteLine(errorMessage);
                            return null;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。必要なアクセス許可がありません。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (ArgumentNullException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスがnullです。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (ArgumentException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (PathTooLongException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (FileNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (NotSupportedException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスの形式が無効です。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }

                return userDatas;
            }
            else
            {
                throw new NotSupportedException("デザインモード時にTwitterAPIを呼び出すことはできません。");
            }
        }

        /// <summary>
        /// フォローされているユーザーのリストを取得します。
        /// </summary>
        /// <returns>フォローされているユーザーのリストを返します。例外発生時はnullを返します。</returns>
        private List<UserData> GetFollowers()
        {
            var userDatas = new List<UserData>();

            if (!Directory.Exists($@"Data\{Tokens.ScreenName}"))
            {
                if (!TryCreateDataDirectory())
                {
                    return null;
                }
            }

            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Followers.data";

            if (File.Exists($@"Data\{Tokens.ScreenName}\{fileName}"))
            {
                // ローカルにデータが存在する場合はデシリアライズして返す
                return GetFollowersFromLocal();
            }
            else
            {
                // ローカルにデータが存在しない場合はTwitterApiを呼び出して取得する
                return GetFollowersFromTwitterApi();
            }
        }

        /// <summary>
        /// ローカルからフォローされているユーザーのリストを取得します。
        /// </summary>
        /// <returns>フォローされているユーザーのリストを返します。例外発生時はnullを返します。</returns>
        private List<UserData> GetFollowersFromLocal()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Followers.data";

            try
            {
                using (var streamReader = File.OpenText($@"Data\{Tokens.ScreenName}\{fileName}"))
                {
                    var userDatas = new List<UserData>();
                    try
                    {
                        userDatas = LZ4MessagePackSerializer.Deserialize<List<UserData>>(streamReader.BaseStream, TypelessContractlessStandardResolver.Instance);
                    }
                    catch (Exception)
                    {
                        var errorMessage = $"{fileName} を開くことに失敗しました。ファイルが壊れているためデータを再取得します。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return GetFollowersFromTwitterApi();
                    }
                    return userDatas;
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// TwitterApiからフォローされているユーザーのリストを取得します。
        /// </summary>
        /// <returns>フォローされているユーザーのリストを返します。例外発生時はnullを返します。</returns>
        private List<UserData> GetFollowersFromTwitterApi()
        {
            if (!IsInDesignMode)
            {
                var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Followers.data";

                var userDatas = new List<UserData>();

                try
                {
                    for (long cursorTmp = -1; cursorTmp != 0;)
                    {
                        var followers = Tokens.Followers.List(
                            user_id => Tokens.UserId,
                            cursor => cursorTmp,
                            count => 200
                            );

                        userDatas.AddRange(followers.Result.Select(user => new UserData { User = user, FollowType = FollowType.NotSet, Favorite = false }));

                        cursorTmp = followers.NextCursor;
                    }
                }
                catch (Exception error)
                {
                    _loggingService.Logs.Add($"フォロワー一覧の取得に失敗しました。{error.Message}");
                    Debug.WriteLine($"フォローワー一覧の取得に失敗しました。{error.Message}");
                    return null;
                }

                try
                {
                    using (var streamWriter = File.CreateText($@"Data\{Tokens.ScreenName}\{fileName}"))
                    {
                        try
                        {
                            LZ4MessagePackSerializer.Serialize(streamWriter.BaseStream, userDatas, TypelessContractlessStandardResolver.Instance);
                        }
                        catch (Exception)
                        {
                            var errorMessage = $"{fileName} の保存に失敗しました。";
                            _loggingService.Logs.Add(errorMessage);
                            Debug.WriteLine(errorMessage);
                            return null;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。必要なアクセス許可がありません。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (ArgumentNullException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスがnullです。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (ArgumentException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (PathTooLongException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (FileNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (NotSupportedException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスの形式が無効です。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }

                return userDatas;
            }
            else
            {
                throw new NotSupportedException("デザインモード時にTwitterAPIを呼び出すことはできません。");
            }
        }

        /// <summary>
        /// ツイートのリストのディクショナリーを取得します。
        /// </summary>
        /// <returns>ツイートのリストのディクショナリーを返します。例外発生時はnullを返します。</returns>
        private Dictionary<long, List<Status>> GetUserTweets()
        {
            var userTweets = new Dictionary<long, List<Status>>();

            if (!Directory.Exists($@"Data\{Tokens.ScreenName}"))
            {
                if (!TryCreateDataDirectory())
                {
                    return null;
                }
            }

            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_UserTweets.data";

            if (File.Exists($@"Data\{Tokens.ScreenName}\{fileName}"))
            {
                // ローカルにデータが存在する場合はデシリアライズして返す
                return GetUserTweetsFromLocal();
            }
            else
            {
                // ローカルにデータが存在しない場合はTwitterApiを呼び出して取得する
                return GetUserTweetsFromTwitterApi();
            }
        }

        /// <summary>
        /// ローカルからツイートのリストのディクショナリーを取得します。
        /// </summary>
        /// <returns>ツイートのリストのディクショナリーを返します。例外発生時はnullを返します。</returns>
        private Dictionary<long, List<Status>> GetUserTweetsFromLocal()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_UserTweets.data";
            try
            {
                using (var streamReader = File.OpenText($@"Data\{Tokens.ScreenName}\{fileName}"))
                {
                    var userTweets = new Dictionary<long, List<Status>>();
                    try
                    {
                        userTweets = LZ4MessagePackSerializer.Deserialize<Dictionary<long, List<Status>>>(streamReader.BaseStream, TypelessContractlessStandardResolver.Instance);
                    }
                    catch (Exception)
                    {
                        var errorMessage = $"{fileName} を開くことに失敗しました。ファイルが壊れているためデータを再取得します。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return GetUserTweetsFromTwitterApi();
                    }
                    return userTweets;
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = $"{fileName} を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        /// <summary>
        /// TwitterApiからツイートのリストのディクショナリーを取得します。
        /// </summary>
        /// <returns>ツイートのリストのディクショナリーを返します。例外発生時はnullを返します。</returns>
        private Dictionary<long, List<Status>> GetUserTweetsFromTwitterApi()
        {
            if (!IsInDesignMode)
            {
                var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_UserTweets.data";

                var userTweets = new Dictionary<long, List<Status>>();

                foreach (var userData in Follows.Union(Followers, new UserDataEqualityComparer()))
                {
                    IEnumerable<Status> statuses;
                    try
                    {
                        statuses = Tokens.Statuses.UserTimeline(user_id => (long)userData.User.Id, count => 200);
                    }
                    catch (Exception error)
                    {
                        var errorMessage = $"@{userData.User.ScreenName}のツイートの取得に失敗しました{error.Message}";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        continue;
                    }
                    userTweets[(long)userData.User.Id] = statuses.ToList();

                    if (userTweets.Count % 100 == 0)
                    {
                        Debug.WriteLine($"Statuses.UserTimelineの呼び出し回数が{userTweets.Count}回に到達しました。");
                    }

                    if (userTweets.Count % 900 == 0)
                    {
                        const string errorMessage = "レートリミットに達したため15分後に再開します。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        Thread.Sleep(new TimeSpan(0, 16, 0));
                    }
                }

                try
                {
                    using (var streamWriter = File.CreateText($@"Data\{Tokens.ScreenName}\{fileName}"))
                    {
                        try
                        {
                            LZ4MessagePackSerializer.Serialize(streamWriter.BaseStream, userTweets, TypelessContractlessStandardResolver.Instance);
                        }
                        catch (Exception)
                        {
                            var errorMessage = $"{fileName} の保存に失敗しました。";
                            _loggingService.Logs.Add(errorMessage);
                            Debug.WriteLine(errorMessage);
                            return null;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。必要なアクセス許可がありません。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (ArgumentNullException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスがnullです。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (ArgumentException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (PathTooLongException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (FileNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }
                catch (NotSupportedException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスの形式が無効です。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                    return null;
                }

                return userTweets;
            }
            else
            {
                throw new NotSupportedException("デザインモード時にTwitterAPIを呼び出すことはできません。");
            }
        }

        /// <summary>
        /// データ保存用のディレクトリを作成します。
        /// </summary>
        /// <returns>ディレクトリの作成に成功したらtrue、失敗したらfalseを返します。</returns>
        private bool TryCreateDataDirectory()
        {
            var directoryName = $@"Data\{Tokens.ScreenName}";

            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = $"{directoryName} の作成に失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = $"{directoryName} の作成に失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (ArgumentException)
            {
                var errorMessage = $"{directoryName} の作成に失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (PathTooLongException)
            {
                var errorMessage = $"{directoryName} の作成に失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = $"{directoryName} の作成に失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (IOException)
            {
                var errorMessage = $"{directoryName} の作成に失敗しました。パスによって指定されたディレクトリはファイルです。またはネットワーク名が不明です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (NotSupportedException)
            {
                var errorMessage = $@"{directoryName} の作成に失敗しました。パスにドライブ ラベル (「C:\」) の一部ではないコロン文字 (:) が含まれています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 自分のユーザーデータを取得します。
        /// </summary>
        /// <returns></returns>
        private User GetMyUserData()
        {
            if (!IsInDesignMode)
            {
                User user;

                try
                {
                    user = Tokens.Users.Show(
                        user_id => Tokens.UserId,
                        include_entities => true
                        );
                }
                catch (Exception)
                {
                    _loggingService.Logs.Add("自分のユーザーデータの取得に失敗しました。");
                    return null;
                }
                return user;
            }
            else
            {
                throw new NotSupportedException("デザインモード時にTwitterAPIを呼び出すことはできません。");
            }
        }
    }
}
