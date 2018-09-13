using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CoreTweet;
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
        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();

        /// <summary>
        /// 現在使用中のアカウント
        /// </summary>
        public Account Current { get; }

        // パブリック関数

        /// <summary>
        /// 指定されたアカウントをコレクションから削除します。
        /// </summary>
        /// <param name="account">アカウント</param>
        public void DeleteAccount(Account account)
        {
            var screenName = account.Tokens.ScreenName;
            Accounts.Remove(account);
            _loggingService.Logs.Add($"@{screenName}を削除しました。");
        }

        // DI注入される変数

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public AccountManager(LoggingService loggingService)
        {
            // DI
            _loggingService = loggingService;

            //Current = new Account(_loggingService);

            //Current.Tokens = Tokens.Create(
            //    "ConsumerKey",
            //    "ConsumerSecret",
            //    "AccessToken",
            //    "AccessTokenSecret",
            //    123456789,
            //    "ScreenName"
            //    );

            // 認証データの読み込み（開発用）

            if (File.Exists(@"Data\Authorization\Authorizations.json"))
            {
                using (var streamReader = File.OpenText(@"Data\Authorization\Authorizations.json"))
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

                        Accounts.Add(account);
                    }
                }

                Current = Accounts.First();
            }
        }

        ~AccountManager()
        {
            // 認証データの保存（開発用）
            var authorizations = new List<Authorization>();

            foreach (var account in Accounts)
            {
                var authorization = new Authorization()
                {
                    ConsumerKey = account.Tokens.ConsumerKey,
                    ConsumerSecret = account.Tokens.ConsumerSecret,
                    AccessToken = account.Tokens.AccessToken,
                    AccessTokenSecret = account.Tokens.AccessTokenSecret,
                    UserId = account.Tokens.UserId,
                    ScreenName = account.Tokens.ScreenName
                };
                authorizations.Add(authorization);
            }

            var directoryName = @"Data\Authorization";

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
    }
}
