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
        public ObservableCollection<User> Follows { get; set; } = new ObservableCollection<User>();

        public ObservableCollection<User> Followers { get; set; } = new ObservableCollection<User>();

        //public Tokens Tokens { get; set; } = new Tokens();
    }
}
