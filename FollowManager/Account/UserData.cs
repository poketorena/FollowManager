using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace FollowManager.Account
{
    public class UserData : BindableBase
    {
        // プロパティ
        private User _user;
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        [JsonIgnore]
        private FollowType _followType;
        [JsonIgnore]
        public FollowType FollowType
        {
            get { return _followType; }
            set { SetProperty(ref _followType, value); }
        }

        [JsonIgnore]
        private bool _favorite;
        [JsonIgnore]
        public bool Favorite
        {
            get { return _favorite; }
            set { SetProperty(ref _favorite, value); }
        }

        // コンストラクタ
        public UserData(User user, FollowType followType, bool favorite)
        {
            User = user;
            FollowType = followType;
            Favorite = favorite;
        }
    }
}
