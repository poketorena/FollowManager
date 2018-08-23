using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using Prism.Mvvm;

namespace FollowManager.Account
{
    public class UserData : BindableBase
    {
        private User _user;
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private FollowType _followType;
        public FollowType FollowType
        {
            get { return _followType; }
            set { SetProperty(ref _followType, value); }
        }

        private bool _favorite;
        public bool Favorite
        {
            get { return _favorite; }
            set { SetProperty(ref _favorite, value); }
        }
    }
}
