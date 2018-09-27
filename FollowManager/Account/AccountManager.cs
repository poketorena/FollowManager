using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CoreTweet;
using FollowManager.Collections.Generic;
using FollowManager.Service;
using Newtonsoft.Json;

namespace FollowManager.Account
{
    /// <summary>
    /// アカウントを管理するクラス
    /// </summary>
    public class AccountManager
    {
        // パブリックプロパティ

        /// <summary>
        /// 登録されているアカウントのリスト
        /// </summary>
        public ObservableDictionary<long, Account> Accounts { get; } = new ObservableDictionary<long, Account>();

        // パブリック関数

        /// <summary>
        /// 指定されたアカウントをコレクションから削除します。
        /// </summary>
        /// <param name="account">アカウント</param>
        public void DeleteAccount(Account account)
        {
            var screenName = account.Tokens.ScreenName;
            try
            {
                Accounts.Remove((long)(account.User.Id));
                _loggingService.Logs.Add($"@{screenName}を削除しました。");
            }
            catch (Exception)
            {
                _loggingService.Logs.Add($"@{screenName}の削除に失敗しました。");
            }
        }

        // DI注入される変数

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public AccountManager(LoggingService loggingService)
        {
            // DI
            _loggingService = loggingService;

            // 認証データの読み込み
            LoadAuthorizationData();
        }

        // デストラクタ

        ~AccountManager()
        {
            SaveAuthorizationData();
        }

        // プライベート関数

        /// <summary>
        /// 認証データを保存します。
        /// </summary>
        private void SaveAuthorizationData()
        {
            try
            {
                var authorizations = new List<Authorization>();

                foreach (var account in Accounts)
                {
                    var authorization = new Authorization()
                    {
                        ConsumerKey = account.Value.Tokens.ConsumerKey,
                        ConsumerSecret = account.Value.Tokens.ConsumerSecret,
                        AccessToken = account.Value.Tokens.AccessToken,
                        AccessTokenSecret = account.Value.Tokens.AccessTokenSecret,
                        UserId = account.Value.Tokens.UserId,
                        ScreenName = account.Value.Tokens.ScreenName
                    };
                    authorizations.Add(authorization);
                }

                const string directoryName = @"Data\Authorization";

                Directory.CreateDirectory(directoryName);

                using (var streamWriter = File.CreateText(@"Data\Authorization\Authorizations.json"))
                {
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    };
                    var jsonSerializer = JsonSerializer.Create(settings);

                    jsonSerializer.Serialize(streamWriter, authorizations);
                }
            }
            catch (Exception)
            {
                const string errorMessage = "認証データの保存に失敗しました。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
            }
        }

        /// <summary>
        /// 認証データを読み込みます。
        /// </summary>
        private void LoadAuthorizationData()
        {
            const string fileName = "Authorizations.json";
            if (File.Exists($@"Data\Authorization\{fileName}"))
            {
                try
                {
                    using (var streamReader = File.OpenText($@"Data\Authorization\{fileName}"))
                    {
                        var authorizations = new List<Authorization>();

                        var settings = new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented
                        };

                        var jsonSerializer = JsonSerializer.Create(settings);

                        authorizations = (List<Authorization>)jsonSerializer.Deserialize(streamReader, typeof(List<Authorization>));

                        foreach (var authorization in authorizations)
                        {
                            var account = new Account(_loggingService)
                            {
                                Tokens = Tokens.Create(
                                    authorization.ConsumerKey,
                                    authorization.ConsumerSecret,
                                    authorization.AccessToken,
                                    authorization.AccessTokenSecret,
                                    authorization.UserId,
                                    authorization.ScreenName
                                    )
                            };

                            Accounts.Add((long)account.User.Id, account);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。必要なアクセス許可がありません。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
                catch (ArgumentNullException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスがnullです。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
                catch (ArgumentException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスは長さ0の文字列か、空白のみで構成されているか、または1つ以上の正しくない文字を含んでいます。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
                catch (PathTooLongException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスかファイル名、またはその両方がシステム定義の最大長を超えています。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
                catch (DirectoryNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスが無効です。マップされていないドライブを指定していませんか？";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
                catch (FileNotFoundException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。指定したパスにファイルが見つかりませんでした。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
                catch (NotSupportedException)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。パスの形式が無効です。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
                catch (Exception)
                {
                    var errorMessage = $"{fileName} を開くことに失敗しました。";
                    _loggingService.Logs.Add(errorMessage);
                    Debug.WriteLine(errorMessage);
                }
            }
        }
    }
}
