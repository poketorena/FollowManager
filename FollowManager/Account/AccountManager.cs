using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreTweet;
using Newtonsoft.Json;

namespace FollowManager.Account
{
    public class AccountManager
    {
        // プロパティ
        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();

        public Account Current { get; }

        // コンストラクタ
        public AccountManager()
        {
            if (File.Exists(@"Test\TestData.json"))
            {
                var jsonData = string.Empty;
                using (var streamReader = new StreamReader(@"Test\TestData.json"))
                {
                    jsonData = streamReader.ReadToEnd();
                }
                Current = JsonConvert.DeserializeObject<Account>(jsonData);
            }

            Current.Tokens = CoreTweet.Tokens.Create(
                Current.Authentication.ConsumerKey,
                Current.Authentication.ConsumerSecret,
                Current.Authentication.AccessToken,
                Current.Authentication.AccessTokenSecret,
                Current.Authentication.UserId,
                Current.Authentication.ScreenName
                );
        }
    }
}
