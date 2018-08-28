namespace FollowManager.Account
{
    /// <summary>
    /// CoreTweetのトークンを作るための認証用データ
    /// </summary>
    public class Authentication
    {
        // パブリックプロパティ

        /// <summary>
        /// コンシューマーキー
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// コンシューマーシークレット
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// アクセストークンシークレット
        /// </summary>
        public string AccessTokenSecret { get; set; }

        /// <summary>
        /// ユーザーId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// スクリーンネーム
        /// </summary>
        public string ScreenName { get; set; }
    }
}
