using System.Reactive.Linq;
using FollowManager.Account;
using FollowManager.AddAccount;
using FollowManager.Service;
using FollowManager.Tab;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;

namespace FollowManager.MainWindow
{
    public class MainWindowViewModelCopy : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのコレクション
        /// </summary>
        public ReactiveCollection<TabItemData> TabItemDatas { get; set; }

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

        // プライベート変数

        private DelegateCommand _openSettingViewCommand;

        private DelegateCommand _openAboutViewCommand;

        private DelegateCommand _openConfigureApiKeyViewCommand;

        private DelegateCommand _openManageAccountViewCommand;

        //private MyInterTabClient _myInterTabClient;

        // DI注入される変数

        private readonly IUnityContainer _unityContainer;

        private readonly AccountManager _accountManager;

        private readonly LoggingService _loggingService;

        private readonly DialogService _dialogService;

        private readonly TabManager _tabManager;

        // コンストラクタ

        public MainWindowViewModelCopy(IUnityContainer unityContainer, AccountManager accountManager, LoggingService loggingService, DialogService dialogService, TabManager tabManager)
        {
            // DI
            _unityContainer = unityContainer;
            _accountManager = accountManager;
            _loggingService = loggingService;
            _dialogService = dialogService;
            _tabManager = tabManager;
        }
    }
}
