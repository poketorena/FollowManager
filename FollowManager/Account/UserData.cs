using CoreTweet;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace FollowManager.Account
{
    /// <summary>
    /// CoreTweetのUserのラッパークラス
    /// </summary>
    public class UserData : BindableBase
    {
        // プロパティ
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        [JsonIgnore]
        public FollowType FollowType
        {
            get { return _followType; }
            set { SetProperty(ref _followType, value); }
        }

        [JsonIgnore]
        public bool Favorite
        {
            get { return _favorite; }
            set { SetProperty(ref _favorite, value); }
        }

        // プライベート変数

        private User _user;

        [JsonIgnore]
        private FollowType _followType;

        [JsonIgnore]
        private bool _favorite;

        // コンストラクタ

        public UserData(User user, FollowType followType, bool favorite)
        {
            User = user;
            FollowType = followType;
            Favorite = favorite;
        }
    }
}
