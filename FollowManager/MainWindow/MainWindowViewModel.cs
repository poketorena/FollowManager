using System.Windows;
using System.Windows.Controls;
using FollowManager.About;
using FollowManager.Account;
using FollowManager.Service;
using FollowManager.Setting;
using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;

namespace FollowManager.MainWindow
{
    public class MainWindowViewModel : BindableBase
    {
        // プロパティ

        // パブリック関数

        // デリゲートコマンド
        private DelegateCommand _settingOpenCommand;
        public DelegateCommand SettingOpenCommand =>
            _settingOpenCommand ?? (_settingOpenCommand = new DelegateCommand(ExecuteSettingOpenCommand));

        private DelegateCommand _aboutOpenCommand;
        public DelegateCommand AboutOpenCommand =>
            _aboutOpenCommand ?? (_aboutOpenCommand = new DelegateCommand(ExecuteAboutOpenCommand));

        // プライベート変数

        // DI注入される変数
        LoggingService _loggingService;
        IRegionManager _reagionManager;
        AccountManager _accountManager;

        // コンストラクタ
        public MainWindowViewModel(LoggingService loggingService, IRegionManager regionManager, AccountManager accountManager)
        {
            _loggingService = loggingService;
            _reagionManager = regionManager;
            _accountManager = accountManager;
        }

        // デストラクタ

        // プライベート関数
        private void ExecuteSettingOpenCommand()
        {
            var window = new SettingView();
            window.Closed += (o, args) => _loggingService.Logs.Add("設定が更新されました。");
            window.Closed += (o, args) => window = null;
            window.ShowDialog();
        }

        private void ExecuteAboutOpenCommand()
        {
            var window = new AboutView();
            window.Closed += (o, args) => _loggingService.Logs.Add("About画面を閉じました。");
            window.Closed += (o, args) => window = null;
            window.ShowDialog();
        }
    }
}
