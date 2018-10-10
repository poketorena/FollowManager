using CoreTweet;
using Prism.Mvvm;

namespace FollowManager.Tab
{
    /// <summary>
    /// タブのデータ
    /// </summary>
    public class TabData : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// ヘッダ
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// タブのId
        /// </summary>
        public string TabId
        {
            get { return _tabId; }
            set { SetProperty(ref _tabId, value); }
        }

        /// <summary>
        /// CoreTweetのトークン
        /// </summary>
        public Tokens Tokens { get; set; }

        // プライベートフィールド

        private string _tabId;
    }
}
