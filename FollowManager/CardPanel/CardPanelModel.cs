using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using FollowManager.Account;
using FollowManager.Service;

namespace FollowManager.CardPanel
{
    public class CardPanelModel
    {
        // プロパティ

        // フィルターのためのキャッシュ
        public ObservableCollection<UserData> OneWay { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Fan { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Mutual { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Inactive { get; set; } = new ObservableCollection<UserData>();

        // パブリック関数
        public void OpenProfile(string screenName)
        {
            // プロフィールページをブラウザで開く
            var url = "https://twitter.com/" + screenName;
            System.Diagnostics.Process.Start(url);
        }

        public async Task ScheduleBlockAndBlockRelease(User user)
        {
            await _accountManager.Current.Tokens.Blocks.CreateAsync(user_id => user.Id);
            _loggingService.Logs.Add($"{user.Name}をブロックしました。");
            await Task.Delay(3000);
            _loggingService.Logs.Add($"{user.Name}のブロックを解除しました。");
            await _accountManager.Current.Tokens.Blocks.DestroyAsync(user_id => user.Id);
        }
        // プライベート変数

        // DI注入される変数
        private readonly AccountManager _accountManager;
        private readonly LoggingService _loggingService;

        // コンストラクタ
        public CardPanelModel(AccountManager accountManager, LoggingService loggingService)
        {
            _accountManager = accountManager;
            _loggingService = loggingService;
        }

        // デストラクタ

        // プライベート関数
    }
}
