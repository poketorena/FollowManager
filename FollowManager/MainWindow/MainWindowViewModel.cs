using FollowManager.Account;
using FollowManager.Service;
using Prism.Commands;
using Prism.Mvvm;

namespace FollowManager.MainWindow
{
    public class MainWindowViewModel : BindableBase
    {
        // プロパティ

        // パブリック関数

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

        // プライベート変数

        private DelegateCommand _openSettingViewCommand;

        private DelegateCommand _openAboutViewCommand;

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

        // デストラクタ

        // プライベート関数
    }
}
