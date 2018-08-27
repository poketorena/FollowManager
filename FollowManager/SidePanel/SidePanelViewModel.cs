using FollowManager.Account;
using FollowManager.FilterAndSort;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using Reactive.Bindings.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace FollowManager.SidePanel
{
    public class SidePanelViewModel : BindableBase
    {
        // プロパティ
        public ReactiveProperty<FilterType> FilterType { get; }
        public ReactiveProperty<SortKeyType> SortKeyType { get; }
        public ReactiveProperty<SortOrderType> SortOrderType { get; }
        // パブリック関数

        // デリゲートコマンド
        private DelegateCommand<string> _changeFilterTypeCommand;
        public DelegateCommand<string> ChangeFilterTypeCommand =>
            // HACK: 非同期処理は要調整
            _changeFilterTypeCommand ?? (_changeFilterTypeCommand = new DelegateCommand<string>(filterType => Task.Run(() => _sidePanelModel.ChangeFilterType(filterType))));

        private DelegateCommand<string> _changeSortKeyTypeCommand;
        public DelegateCommand<string> ChangeSortKeyTypeCommand =>
            _changeSortKeyTypeCommand ?? (_changeSortKeyTypeCommand = new DelegateCommand<string>(_sidePanelModel.ChangeSortKeyType));

        private DelegateCommand<string> _changeSortOrderTypeCommand;
        public DelegateCommand<string> ChangeSortOrderTypeCommand =>
            _changeSortOrderTypeCommand ?? (_changeSortOrderTypeCommand = new DelegateCommand<string>(_sidePanelModel.ChangeSortOrderType));

        // インタラクションリクエスト

        // プライベート変数

        // DI注入される変数
        private readonly AccountManager _accountManager;
        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ
        public SidePanelViewModel(AccountManager accountManager, SidePanelModel sidePanelModel)
        {
            _accountManager = accountManager;
            _sidePanelModel = sidePanelModel;

            FilterType = _sidePanelModel.FilterAndSortOption.ToReactivePropertyAsSynchronized(x => x.FilterType);

            SortKeyType = _sidePanelModel.FilterAndSortOption.ToReactivePropertyAsSynchronized(x => x.SortKeyType);

            SortOrderType = _sidePanelModel.FilterAndSortOption.ToReactivePropertyAsSynchronized(x => x.SortOrderType);
        }

        // デストラクタ

        // プライベート関数
    }
}
