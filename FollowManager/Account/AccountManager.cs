using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreTweet;
using FollowManager.Service;
using Newtonsoft.Json;

namespace FollowManager.Account
{
    public class AccountManager
    {
        // プロパティ
        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();

        public Account Current { get; }

        // コンストラクタ
        public AccountManager(LoggingService loggingService)
        {
            Current = new Account(loggingService);

            Current.Tokens = Tokens.Create(
                "ConsumerKey",
                "ConsumerSecret",
                "AccessToken",
                "AccessTokenSecret",
                123456789,
                "ScreenName"
                );
        }
    }
}
