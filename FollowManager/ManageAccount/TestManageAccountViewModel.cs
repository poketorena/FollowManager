using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace FollowManager.ManageAccount
{
    public class TestManageAccountViewModel
    {
        public ReadOnlyReactiveCollection<Account.TestAccount> Accounts { get; }

        public TestManageAccountViewModel()
        {
            var accounts = Observable
                .Range(0, 20)
                .Select(_ => new Account.TestAccount
                {
                    Tokens = new CoreTweet.Tokens
                    {
                        ScreenName = "science507"
                    },
                    User = new CoreTweet.User
                    {
                        Name = "ポケトレーナー",
                        ProfileImageUrlHttps = "https://pbs.twimg.com/profile_images/815189933124042756/xVkaYdkM_normal.jpg"
                    }
                });

            Accounts = accounts.ToReadOnlyReactiveCollection();
        }
    }
}
