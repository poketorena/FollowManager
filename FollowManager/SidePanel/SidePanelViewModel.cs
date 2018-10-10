using System.Reactive.Disposables;
using FollowManager.Account;
using FollowManager.EventAggregator;
using FollowManager.FilterAndSort;
using FollowManager.MultiBinding.CommandAndConverterParameter;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.SidePanel
{
    public class SidePanelViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのId
        /// </summary>
        public ReactiveProperty<string> TabId { get; set; } = new ReactiveProperty<string>();

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

        // コマンド

        /// <summary>
        /// フィルタを変更するコマンド
        /// </summary>
        public DelegateCommand<object> ChangeFilterTypeCommand =>
            // HACK: 非同期処理は要調整
            _changeFilterTypeCommand ?? (_changeFilterTypeCommand = new DelegateCommand<object>(filterRequest => _sidePanelModel.ChangeFilterType((FilterRequest)filterRequest)));

        /// <summary>
        /// ソートキーを変更するコマンド
        /// </summary>
        public DelegateCommand<object> ChangeSortKeyTypeCommand =>
            _changeSortKeyTypeCommand ?? (_changeSortKeyTypeCommand = new DelegateCommand<object>(sortKeyRequest => _sidePanelModel.ChangeSortKeyType((SortKeyRequest)sortKeyRequest)));

        /// <summary>
        /// ソート順を変更するコマンド
        /// </summary>
        public DelegateCommand<object> ChangeSortOrderTypeCommand =>
            _changeSortOrderTypeCommand ?? (_changeSortOrderTypeCommand = new DelegateCommand<object>(sortOrderRequest => _sidePanelModel.ChangeSortOrderType((SortOrderRequest)sortOrderRequest)));

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベートフィールド

        private DelegateCommand<object> _changeFilterTypeCommand;

        private DelegateCommand<object> _changeSortKeyTypeCommand;

        private DelegateCommand<object> _changeSortOrderTypeCommand;

        // DI注入されるフィールド

        private readonly IEventAggregator _eventAggregator;

        private readonly AccountManager _accountManager;

        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ

        public SidePanelViewModel(IEventAggregator eventAggregator, AccountManager accountManager, SidePanelModel sidePanelModel)
        {
            // DI
            _eventAggregator = eventAggregator;
            _accountManager = accountManager;
            _sidePanelModel = sidePanelModel;

            // フィルタを購読して現在使用しているフィルタを更新する
            FilterType = _sidePanelModel
                .FilterAndSortOption
                .ToReactivePropertyAsSynchronized(x => x.FilterType)
                .AddTo(Disposables);

            // ソートキーを購読して現在使用しているソートキーを更新する
            SortKeyType = _sidePanelModel
                .FilterAndSortOption
                .ToReactivePropertyAsSynchronized(x => x.SortKeyType)
                .AddTo(Disposables);

            // ソート順を購読して現在使用しているソート順を更新する
            SortOrderType = _sidePanelModel
                .FilterAndSortOption
                .ToReactivePropertyAsSynchronized(x => x.SortOrderType)
                .AddTo(Disposables);

            // タブが削除されたらリソースを開放する
            _eventAggregator
                .GetEvent<TabRemovedEvent>()
                .Subscribe(_ => Disposables.Dispose(), ThreadOption.PublisherThread, false, tabRemovedEventArgs => tabRemovedEventArgs.TabId == TabId.Value)
                .AddTo(Disposables);
        }
    }
}
