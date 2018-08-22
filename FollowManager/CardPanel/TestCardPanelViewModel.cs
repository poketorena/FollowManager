using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using Newtonsoft.Json;
using Reactive.Bindings;

namespace FollowManager.CardPanel
{
    public class TestCardPanelViewModel
    {
        // プロパティ
        public ReactiveCollection<User> Follows { get; } = new ReactiveCollection<User>();

        // コンストラクタ
        public TestCardPanelViewModel()
        {
            var users = Observable
            .Range(0, 20)
            .Select(x => new User
            {
                ProfileImageUrlHttps = "https://pbs.twimg.com/profile_images/815189933124042756/xVkaYdkM_normal.jpg",
                ProfileBannerUrl = "https://pbs.twimg.com/profile_banners/1302791522/1364285191/1500x500",
                Description = "ソフトウェアエンジニアになりたい大学生/初心者C++er/C#/WPF/Xamarin/CTRL3だよ",
                ScreenName = "science507",
                Name = "ポケトレーナー"
            });

            Follows = new ReactiveCollection<User>(users);
        }
    }
}
