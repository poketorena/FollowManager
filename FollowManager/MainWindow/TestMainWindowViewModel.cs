using System.Linq;
using System.Reactive.Linq;
using FollowManager.Tab;
using Reactive.Bindings;

namespace FollowManager.MainWindow
{
    public class TestMainWindowViewModel
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのコレクション
        /// </summary>
        public ReactiveCollection<TabData> TabDatas { get; set; }

        // コンストラクタ

        /// <summary>
        /// XAMLデザイナー用コンストラクタ
        /// </summary>
        public TestMainWindowViewModel()
        {
            var tabDatas = Observable
                .Range(0, 5)
                .Select(_ => new TabData
                {
                    Header = "@science507",
                });

            TabDatas = tabDatas.ToReactiveCollection();
        }
    }
}
