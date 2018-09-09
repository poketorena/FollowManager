using CoreTweet;

namespace FollowManager.Account
{
    /// <summary>
    /// Accountのモック
    /// </summary>
    public class TestAccount
    {
        /// <summary>
        /// CoreTweetのトークン
        /// </summary>
        public Tokens Tokens { get; set; }

        /// <summary>
        /// 自分のUserデータ
        /// </summary>
        public User User { get; set; }
    }
}
