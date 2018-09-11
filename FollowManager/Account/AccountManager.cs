using System.Collections.ObjectModel;
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

            Current = new Account(_loggingService);

            Current.Tokens = Tokens.Create(
                "ConsumerKey",
                "ConsumerSecret",
                "AccessToken",
                "AccessTokenSecret",
                123456789,
                "ScreenName"
                );
        }

        ~AccountManager()
        {

        }
    }
}
