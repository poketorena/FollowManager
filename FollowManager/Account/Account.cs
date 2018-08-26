﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreTweet;
using FollowManager.FilterAndSort;
using FollowManager.Service;
using Newtonsoft.Json;

namespace FollowManager.Account
{
    public class Account
    {
        // プロパティ
        private List<UserData> _follows;
        public List<UserData> Follows =>
            _follows ?? (_follows = GetFollows());

        private List<UserData> _followers;
        public List<UserData> Followers =>
            _followers ?? (_followers = GetFollowers());

        private Dictionary<long, List<Status>> _userTweets;
        public Dictionary<long, List<Status>> UserTweets =>
            _userTweets ?? (_userTweets = GetUserTweets());

        public FilterAndSortOption FilterAndSortOption { get; set; } = new FilterAndSortOption();

        public Tokens Tokens { get; set; } = new Tokens();

        // 認証用データ
        public Authentication Authentication { get; set; } = new Authentication();

        // パブリック関数

        // プライベート変数

        // DI注入される変数
        private readonly LoggingService _loggingService;

        // コンストラクタ
        public Account(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }
        // デストラクタ

        // プライベート関数
        private List<UserData> GetFollows()
        {
            var userDatas = new List<UserData>();

            if (!Directory.Exists($@"Data\{Tokens.ScreenName}"))
            {
                if (!CreateDataDirectory())
                {
                    return null;
                }
            }

            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Follows.json";

            if (File.Exists($@"Data\{Tokens.ScreenName}\" + fileName))
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

        private List<UserData> GetFollowsFromLocal()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Follows.json";

            try
            {
                using (var streamReader = File.OpenText($@"Data\{Tokens.ScreenName}\" + fileName))
                {
                    var userDatas = new List<UserData>();
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    };
                    var jsonSerializer = JsonSerializer.Create(settings);
                    try
                    {
                        userDatas = (List<UserData>)jsonSerializer.Deserialize(streamReader, typeof(List<UserData>));
                    }
                    catch (Exception)
                    {
                        var errorMessage = fileName + " を開くことに失敗しました。ファイルが壊れているためデータを再取得します。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return GetFollowsFromTwitterApi();
                    }
                    return userDatas;
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        private List<UserData> GetFollowsFromTwitterApi()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Follows.json";

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

                    userDatas.AddRange(friends.Result.Select(user => new UserData(user, FollowType.NotSet, false)));

                    cursorTmp = friends.NextCursor;
                }
            }
            catch (Exception error)
            {
                _loggingService.Logs.Add("フォロー一覧の取得に失敗しました。" + error.Message);
                Debug.WriteLine("フォロー一覧の取得に失敗しました。" + error.Message);
                return null;
            }

            try
            {
                using (var streamWriter = File.CreateText($@"Data\{Tokens.ScreenName}\" + fileName))
                {
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    };
                    var jsonSerializer = JsonSerializer.Create(settings);
                    try
                    {
                        jsonSerializer.Serialize(streamWriter, userDatas);
                    }
                    catch (Exception)
                    {
                        var errorMessage = fileName + " の保存に失敗しました。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return null;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }

            return userDatas;
        }

        private List<UserData> GetFollowers()
        {
            var userDatas = new List<UserData>();

            if (!Directory.Exists($@"Data\{Tokens.ScreenName}"))
            {
                if (!CreateDataDirectory())
                {
                    return null;
                }
            }

            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Followers.json";

            if (File.Exists($@"Data\{Tokens.ScreenName}\" + fileName))
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

        private List<UserData> GetFollowersFromLocal()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Followers.json";

            try
            {
                using (var streamReader = File.OpenText($@"Data\{Tokens.ScreenName}\" + fileName))
                {
                    var userDatas = new List<UserData>();
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    };
                    var jsonSerializer = JsonSerializer.Create(settings);
                    try
                    {
                        userDatas = (List<UserData>)jsonSerializer.Deserialize(streamReader, typeof(List<UserData>));
                    }
                    catch (Exception)
                    {
                        var errorMessage = fileName + " を開くことに失敗しました。ファイルが壊れているためデータを再取得します。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return GetFollowersFromTwitterApi();
                    }
                    return userDatas;
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        private List<UserData> GetFollowersFromTwitterApi()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_Followers.json";

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

                    userDatas.AddRange(followers.Result.Select(user => new UserData(user, FollowType.NotSet, false)));

                    cursorTmp = followers.NextCursor;
                }
            }
            catch (Exception error)
            {
                _loggingService.Logs.Add("フォロワー一覧の取得に失敗しました。" + error.Message);
                Debug.WriteLine("フォローワー一覧の取得に失敗しました。" + error.Message);
                return null;
            }

            try
            {
                using (var streamWriter = File.CreateText($@"Data\{Tokens.ScreenName}\" + fileName))
                {
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    };
                    var jsonSerializer = JsonSerializer.Create(settings);
                    try
                    {
                        jsonSerializer.Serialize(streamWriter, userDatas);
                    }
                    catch (Exception)
                    {
                        var errorMessage = fileName + " の保存に失敗しました。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return null;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }

            return userDatas;
        }

        private Dictionary<long, List<Status>> GetUserTweets()
        {
            var userTweets = new Dictionary<long, List<Status>>();

            if (!Directory.Exists($@"Data\{Tokens.ScreenName}"))
            {
                if (!CreateDataDirectory())
                {
                    return null;
                }
            }

            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_UserTweets.json";

            if (File.Exists($@"Data\{Tokens.ScreenName}\" + fileName))
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

        private Dictionary<long, List<Status>> GetUserTweetsFromLocal()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_UserTweets.json";
            try
            {
                using (var streamReader = File.OpenText($@"Data\{Tokens.ScreenName}\" + fileName))
                {
                    var userTweets = new Dictionary<long, List<Status>>();
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    };
                    var jsonSerializer = JsonSerializer.Create(settings);
                    try
                    {
                        userTweets = (Dictionary<long, List<Status>>)jsonSerializer.Deserialize(streamReader, typeof(Dictionary<long, List<Status>>));
                    }
                    catch (Exception)
                    {
                        var errorMessage = fileName + " を開くことに失敗しました。ファイルが壊れているためデータを再取得します。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return GetUserTweetsFromTwitterApi();
                    }
                    return userTweets;
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
        }

        private Dictionary<long, List<Status>> GetUserTweetsFromTwitterApi()
        {
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_UserTweets.json";

            var userTweets = new Dictionary<long, List<Status>>();

            foreach (var userData in Follows.Union(Followers))
            {
                IEnumerable<Status> statuses;
                try
                {
                    statuses = Tokens.Statuses.UserTimeline(user_id => (long)userData.User.Id, count => 200);
                }
                catch (Exception error)
                {
                    _loggingService.Logs.Add("@" + userData.User.ScreenName + "のツイートの取得に失敗しました。" + error.Message);
                    Debug.WriteLine("@" + userData.User.ScreenName + "のツイートの取得に失敗しました。" + error.Message);
                    continue;
                }
                userTweets[(long)userData.User.Id] = statuses.ToList();

                if (userTweets.Count % 100 == 0)
                {
                    Debug.WriteLine("Statuses.UserTimelineの呼び出し回数が" + userTweets.Count + "回に到達しました。");
                }

                if (userTweets.Count % 900 == 0)
                {
                    _loggingService.Logs.Add("レートリミットに達したため15分後に再開します。");
                    Debug.WriteLine("レートリミットに達したため15分後に再開します。");
                    Thread.Sleep(new TimeSpan(0, 16, 0));
                }
            }

            try
            {
                using (var streamWriter = File.CreateText($@"Data\{Tokens.ScreenName}\" + fileName))
                {
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    };
                    var jsonSerializer = JsonSerializer.Create(settings);
                    try
                    {
                        jsonSerializer.Serialize(streamWriter, userTweets);
                    }
                    catch (Exception)
                    {
                        var errorMessage = fileName + " の保存に失敗しました。";
                        _loggingService.Logs.Add(errorMessage);
                        Debug.WriteLine(errorMessage);
                        return null;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (ArgumentException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (PathTooLongException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (FileNotFoundException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }
            catch (NotSupportedException)
            {
                var errorMessage = fileName + " を開くことに失敗しました。パスの形式が無効です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return null;
            }

            return userTweets;
        }

        /// <summary>
        /// データ保存用のディレクトリを作成します。
        /// ディレクトリの作成に成功したらtrue、失敗したらfalseを返します。
        /// </summary>
        /// <returns>ディレクトリの作成に成功したかどうか</returns>
        private bool CreateDataDirectory()
        {
            var directoryName = $@"Data\{Tokens.ScreenName}";

            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (UnauthorizedAccessException)
            {
                var errorMessage = directoryName + " の作成に失敗しました。必要なアクセス許可がありません。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (ArgumentNullException)
            {
                var errorMessage = directoryName + " の作成に失敗しました。指定したパスがnullです。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (ArgumentException)
            {
                var errorMessage = directoryName + " の作成に失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (PathTooLongException)
            {
                var errorMessage = directoryName + " の作成に失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                var errorMessage = directoryName + " の作成に失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (IOException)
            {
                var errorMessage = directoryName + " の作成に失敗しました。パスによって指定されたディレクトリはファイルです。またはネットワーク名が不明です。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            catch (NotSupportedException)
            {
                var errorMessage = directoryName + @" の作成に失敗しました。パスにドライブ ラベル (「C:\」) の一部ではないコロン文字 (:) が含まれています。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
                return false;
            }
            return true;
        }
    }
}
