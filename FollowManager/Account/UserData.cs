using CoreTweet;
using MessagePack;
using Prism.Mvvm;

namespace FollowManager.Account
{
    /// <summary>
    /// CoreTweetのUserのラッパークラス
    /// </summary>
    [MessagePackObject]
    public class UserData : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// CoreTweetのUser
        /// </summary>
        [Key(0)]
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        /// <summary>
        /// 自分とユーザーの関係
        /// </summary>
        [IgnoreMember]
        public FollowType FollowType
        {
            get { return _followType; }
            set { SetProperty(ref _followType, value); }
        }

        /// <summary>
        /// お気に入り
        /// </summary>
        [IgnoreMember]
        public bool Favorite
        {
            get { return _favorite; }
            set { SetProperty(ref _favorite, value); }
        }

        // プライベートフィールド

        [Key(1)]
        private User _user;

        [IgnoreMember]
        private FollowType _followType;

        [IgnoreMember]
        private bool _favorite;
    }
}
