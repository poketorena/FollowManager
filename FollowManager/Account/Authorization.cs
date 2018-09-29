using MessagePack;

namespace FollowManager.Account
{
    /// <summary>
    /// CoreTweetのトークンを作るための認証用データ
    /// </summary>
    [MessagePackObject]
    public class Authorization
    {
        // パブリックプロパティ

        /// <summary>
        /// コンシューマーキー
        /// </summary>
        [Key(0)]
        public string ConsumerKey { get; set; }

        /// <summary>
        /// コンシューマーシークレット
        /// </summary>
        [Key(1)]
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// アクセストークン
        /// </summary>
        [Key(2)]
        public string AccessToken { get; set; }

        /// <summary>
        /// アクセストークンシークレット
        /// </summary>
        [Key(3)]
        public string AccessTokenSecret { get; set; }

        /// <summary>
        /// ユーザーId
        /// </summary>
        [Key(4)]
        public long UserId { get; set; }

        /// <summary>
        /// スクリーンネーム
        /// </summary>
        [Key(5)]
        public string ScreenName { get; set; }
    }
}
