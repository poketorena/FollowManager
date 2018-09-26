using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Dragablz;
using FollowManager.Account;
using FollowManager.Dispose;
using FollowManager.Service;
using FollowManager.Tab;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.MainWindow
{
    public class MainWindowViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのコレクション
        /// </summary>
        public ReactiveProperty<ObservableCollection<TabData>> TabDatas { get; private set; }

        // デリゲートコマンド

        /// <summary>
        /// 設定画面を開くコマンド
        /// </summary>
        public DelegateCommand OpenSettingViewCommand =>
            _openSettingViewCommand ?? (_openSettingViewCommand = new DelegateCommand(_dialogService.OpenSettingView));

        /// <summary>
        /// About画面を開くコマンド
        /// </summary>
        public DelegateCommand OpenAboutViewCommand =>
            _openAboutViewCommand ?? (_openAboutViewCommand = new DelegateCommand(_dialogService.OpenAboutView));

        /// <summary>
        /// アカウント追加画面を開くコマンド
        /// </summary>
        public DelegateCommand OpenConfigureApiKeyViewCommand =>
            _openConfigureApiKeyViewCommand ?? (_openConfigureApiKeyViewCommand = new DelegateCommand(_dialogService.OpenConfigureApiKeyView));

        /// <summary>
        /// アカウント管理画面を開くコマンド
        /// </summary>
        public DelegateCommand OpenManageAccountViewCommand =>
            _openManageAccountViewCommand ?? (_openManageAccountViewCommand = new DelegateCommand(_dialogService.OpenManageAccountView));

        /// <summary>
        /// 新規アカウントタブを開くコマンド
        /// </summary>
        public DelegateCommand OpenAddAccountTabViewCommand =>
            _openAddAccountTabViewCommand ?? (_openAddAccountTabViewCommand = new DelegateCommand(_dialogService.OpenAddAccountTabView));

        /// <summary>
        /// タブを閉じるコマンド
        /// </summary>
        public DelegateCommand<TabData> CloseTabCommand =>
            _closeTabCommand ?? (_closeTabCommand = new DelegateCommand<TabData>(_tabManager.CloseTab));

        /// <summary>
        /// 全てのタブを閉じるコマンド
        /// </summary>
        public DelegateCommand CloseAllTabsCommand =>
            _closeAllTabsCommand ?? (_closeAllTabsCommand = new DelegateCommand(_tabManager.CloseAllTabs));

        /// <summary>
        /// このタブ以外をすべて閉じるコマンド
        /// </summary>
        public DelegateCommand<TabData> CloseAllTabsExceptThisTabCommand =>
            _closeAllTabsExceptThisTabCommand ?? (_closeAllTabsExceptThisTabCommand = new DelegateCommand<TabData>(_tabManager.CloseAllTabsExceptThisTab));

        /// <summary>
        /// クリックでタブを閉じるときに呼ばれるメソッド
        /// </summary>
        public ItemActionCallback ClosingTabItemHandler
            => _tabManager.ClosingTabItemHandlerImpl;

        // プライベートプロパティ

        /// <summary>
        /// IDisposableのコレクション
        /// </summary>
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        private DelegateCommand _openSettingViewCommand;

        private DelegateCommand _openAboutViewCommand;

        private DelegateCommand _openConfigureApiKeyViewCommand;

        private DelegateCommand _openManageAccountViewCommand;

        private DelegateCommand _openAddAccountTabViewCommand;

        private DelegateCommand<TabData> _closeTabCommand;

        private DelegateCommand _closeAllTabsCommand;

        private DelegateCommand<TabData> _closeAllTabsExceptThisTabCommand;

        // DI注入される変数

        private readonly IUnityContainer _unityContainer;

        private readonly AccountManager _accountManager;

        private readonly LoggingService _loggingService;

        private readonly DialogService _dialogService;

        private readonly TabManager _tabManager;

        // コンストラクタ

        public MainWindowViewModel(IUnityContainer unityContainer, AccountManager accountManager, LoggingService loggingService, DialogService dialogService, TabManager tabManager)
        {
            // DI
            _unityContainer = unityContainer;
            _accountManager = accountManager;
            _loggingService = loggingService;
            _dialogService = dialogService;
            _tabManager = tabManager;

            // モデルのタブのコレクションの変更を購読してタブのコレクションを更新する
            TabDatas = _tabManager
                .ToReactivePropertyAsSynchronized(model => model.TabDatas)
                .AddTo(DisposeManager.Instance.Disposables);

            //var tabItemDatas = Observable
            //    .Range(0, 5)
            //    .Select(num => new TabItemData
            //    {
            //        Header = "@science50" + (num++).ToString(),
            //    });

            //TabItemDatas = tabItemDatas.ToReactiveCollection();
        }
    }
}
