using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ReactiveCollection<TabItemData> TabItemDatas { get; set; }

        // コンストラクタ

        public TestMainWindowViewModel()
        {
            var tabItemDatas = Observable
                .Range(0, 5)
                .Select(_ => new TabItemData
                {
                    Header = "@science507",
                });

            TabItemDatas = tabItemDatas.ToReactiveCollection();
        }
    }
}
