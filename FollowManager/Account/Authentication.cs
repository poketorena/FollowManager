namespace FollowManager.Account
{
    /// <summary>
    /// CoreTweetのトークンを作るための認証用データ
    /// </summary>
    public class Authentication
    {
        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public long UserId { get; set; }

        public string ScreenName { get; set; }
    }
}
