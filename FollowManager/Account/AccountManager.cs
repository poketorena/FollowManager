using System;
using System.Collections.Generic;
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

                        Accounts.Add((long)account.User.Id,account);
                    }
                }
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
                    ConsumerKey = account.Value.Tokens.ConsumerKey,
                    ConsumerSecret = account.Value.Tokens.ConsumerSecret,
                    AccessToken = account.Value.Tokens.AccessToken,
                    AccessTokenSecret = account.Value.Tokens.AccessTokenSecret,
                    UserId = account.Value.Tokens.UserId,
                    ScreenName = account.Value.Tokens.ScreenName
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
