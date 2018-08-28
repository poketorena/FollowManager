using System.Threading.Tasks;
using FollowManager.Account;
using FollowManager.FilterAndSort;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.SidePanel
{
    public class SidePanelViewModel : BindableBase
    {
        // プロパティ

        /// <summary>
        /// 現在使用しているフィルタ
        /// </summary>
        public ReactiveProperty<FilterType> FilterType { get; }

        /// <summary>
        /// 現在使用しているソートキー
        /// </summary>
        public ReactiveProperty<SortKeyType> SortKeyType { get; }

        /// <summary>
        /// 現在使用しているソート順
        /// </summary>
        public ReactiveProperty<SortOrderType> SortOrderType { get; }

        // パブリック関数

        // デリゲートコマンド

        /// <summary>
        /// フィルタを変更するコマンド
        /// </summary>
        public DelegateCommand<string> ChangeFilterTypeCommand =>
            // HACK: 非同期処理は要調整
            _changeFilterTypeCommand ?? (_changeFilterTypeCommand = new DelegateCommand<string>(filterType => Task.Run(() => _sidePanelModel.ChangeFilterType(filterType))));

        /// <summary>
        /// ソートキーを変更するコマンド
        /// </summary>
        public DelegateCommand<string> ChangeSortKeyTypeCommand =>
            _changeSortKeyTypeCommand ?? (_changeSortKeyTypeCommand = new DelegateCommand<string>(_sidePanelModel.ChangeSortKeyType));

        /// <summary>
        /// ソート順を変更するコマンド
        /// </summary>
        public DelegateCommand<string> ChangeSortOrderTypeCommand =>
            _changeSortOrderTypeCommand ?? (_changeSortOrderTypeCommand = new DelegateCommand<string>(_sidePanelModel.ChangeSortOrderType));

        // インタラクションリクエスト

        // プライベート変数

        private DelegateCommand<string> _changeFilterTypeCommand;

        private DelegateCommand<string> _changeSortKeyTypeCommand;

        private DelegateCommand<string> _changeSortOrderTypeCommand;

        // DI注入される変数

        private readonly AccountManager _accountManager;

        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ

        public SidePanelViewModel(AccountManager accountManager, SidePanelModel sidePanelModel)
        {
            // DI
            _accountManager = accountManager;
            _sidePanelModel = sidePanelModel;

            // フィルタを購読して現在使用しているフィルタを更新する
            FilterType = _sidePanelModel
                .FilterAndSortOption
                .ToReactivePropertyAsSynchronized(x => x.FilterType);

            // ソートキーを購読して現在使用しているソートキーを更新する
            SortKeyType = _sidePanelModel
                .FilterAndSortOption
                .ToReactivePropertyAsSynchronized(x => x.SortKeyType);

            // ソート順を購読して現在使用しているソート順を更新する
            SortOrderType = _sidePanelModel
                .FilterAndSortOption
                .ToReactivePropertyAsSynchronized(x => x.SortOrderType);
        }

        // デストラクタ

        // プライベート関数
    }
}
