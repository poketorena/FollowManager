using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;

namespace FollowManager.Account
{
    public class Account
    {
        // プロパティ
        public ObservableCollection<UserData> Follows { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Followers { get; set; } = new ObservableCollection<UserData>();

        //public Tokens Tokens { get; set; } = new Tokens();
    }
}
