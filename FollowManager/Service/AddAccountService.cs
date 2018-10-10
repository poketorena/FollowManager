using System;
using System.ComponentModel;
using System.Windows;
using CoreTweet;
using FollowManager.Account;
using Microsoft.Practices.Unity;

namespace FollowManager.Service
{
    public class AddAccountService
    {
        // パブリックプロパティ

        /// <summary>
        /// XAML編集中にAPIを呼び出すのを防ぐフラグ
        /// </summary>
        public static bool IsInDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        // パブリックメソッド

        /// <summary>
        /// Twitterの認証ページを規定のブラウザで開きます。
        /// </summary>
        /// <param name="consumerKey">コンシューマーキー</param>
        /// <param name="consumerSecret">コンシューマーシークレット</param>
        public void OpenAuthorizeUrl(string consumerKey, string consumerSecret)
        {
            try
            {
                if (!IsInDesignMode)
                {
                    _session = OAuth.Authorize(consumerKey, consumerSecret);
                    var url = _session.AuthorizeUri;
                    System.Diagnostics.Process.Start(url.AbsoluteUri);
                }
            }
            catch (Exception)
            {
                _loggingService.Logs.Add("アカウントの追加に失敗しました。");
                DeleteSession();
            }
        }

        /// <summary>
        /// Pinコードを使ってアクセストークンを生成し、アカウントを追加します。
        /// </summary>
        /// <param name="pincode">Pinコード</param>
        public void ConfigureAccessTokens(string pincode)
        {
            try
            {
                if (!IsInDesignMode)
                {
                    var tokens = OAuth.GetTokens(_session, pincode);
                    var account = _unityContainer.Resolve<Account.Account>();
                    account.Tokens = tokens;
                    _accountManager.Accounts.Add((long)account.User.Id, account);
                    DeleteSession();
                    _loggingService.Logs.Add("アカウントの追加に成功しました。");
                }
            }
            catch (Exception)
            {
                _loggingService.Logs.Add("アカウントの追加に失敗しました。");
                DeleteSession();
            }
        }

        // プライベートフィールド

        private OAuth.OAuthSession _session;

        // DI注入されるフィールド

        private readonly IUnityContainer _unityContainer;

        private readonly LoggingService _loggingService;

        private readonly AccountManager _accountManager;

        // コンストラクタ

        public AddAccountService(IUnityContainer unityContainer, LoggingService loggingService, AccountManager accountManager)
        {
            // DI
            _unityContainer = unityContainer;
            _loggingService = loggingService;
            _accountManager = accountManager;
        }

        // プライベートメソッド

        /// <summary>
        /// 認証で使ったセッションを消去します。
        /// </summary>
        private void DeleteSession()
        {
            _session = null;
        }
    }
}
