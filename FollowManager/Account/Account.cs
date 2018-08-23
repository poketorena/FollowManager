using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using FollowManager.FilterAndSort;
using Newtonsoft.Json;

namespace FollowManager.Account
{
    public class Account
    {
        // プロパティ
        public ObservableCollection<UserData> Follows { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Followers { get; set; } = new ObservableCollection<UserData>();

        // フィルターのためのキャッシュ
        public ObservableCollection<UserData> OneWay { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Fan { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Mutual { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> Inactive { get; set; } = new ObservableCollection<UserData>();

        [JsonIgnore]
        public FilterAndSortOption FilterAndSortOption { get; set; } = new FilterAndSortOption();

        [JsonIgnore]
        public Tokens Tokens { get; set; } = new Tokens();

        // 認証用データ
        public Authentication Authentication { get; set; } = new Authentication();
    }
}
