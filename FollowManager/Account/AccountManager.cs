using System.Collections.Generic;
using CoreTweet;
using FollowManager.Service;

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
        public List<Account> Accounts { get; } = new List<Account>();

        /// <summary>
        /// 現在使用中のアカウント
        /// </summary>
        public Account Current { get; }

        // コンストラクタ

        public AccountManager(LoggingService loggingService)
        {
            Current = new Account(loggingService);

            Current.Tokens = Tokens.Create(
                "ConsumerKey",
                "ConsumerSecret",
                "AccessToken",
                "AccessTokenSecret",
                123456789,
                "ScreenName"
                );
        }
    }
}
