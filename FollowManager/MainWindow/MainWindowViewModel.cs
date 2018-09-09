using FollowManager.Account;
using FollowManager.AddAccount;
using FollowManager.Service;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;

namespace FollowManager.MainWindow
{
    public class MainWindowViewModel : BindableBase
    {
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

        // DI注入される変数

        private readonly AccountManager _accountManager;

        private readonly LoggingService _loggingService;

        private readonly DialogService _dialogService;

        // コンストラクタ

        public MainWindowViewModel(AccountManager accountManager, LoggingService loggingService, DialogService dialogService)
        {
            // DI
            _accountManager = accountManager;
            _loggingService = loggingService;
            _dialogService = dialogService;
        }
    }
}
