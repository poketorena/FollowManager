using System.Linq;
using System.Reactive.Linq;
using CoreTweet;
using FollowManager.Account;
using Reactive.Bindings;

namespace FollowManager.CardPanel
{
    public class TestCardPanelViewModel
    {
        // パブリックプロパティ

        public ReactiveCollection<UserData> UserDatas { get; } = new ReactiveCollection<UserData>();

        // コンストラクタ

        public TestCardPanelViewModel()
        {
            var users = Observable
            .Range(0, 20)
            .Select(_ => new UserData
            (
                new User
                {
                    ProfileImageUrlHttps = "https://pbs.twimg.com/profile_images/815189933124042756/xVkaYdkM_normal.jpg",
                    ProfileBannerUrl = "https://pbs.twimg.com/profile_banners/1302791522/1364285191/1500x500",
                    Description = "ソフトウェアエンジニアになりたい大学生/初心者C++er/C#/WPF/Xamarin/CTRL3だよ",
                    ScreenName = "science507",
                    Name = "ポケトレーナー"
                },
                FollowType.OneWay,
                true
            ));

            UserDatas = new ReactiveCollection<UserData>(users);
        }
    }
}
