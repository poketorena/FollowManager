using System.Windows;
using System.Windows.Controls;
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

        // プライベート変数

        // DI注入される変数
        LoggingService _loggingService;
        IRegionManager _reagionManager;

        // コンストラクタ
        public MainWindowViewModel(LoggingService loggingService, IRegionManager regionManager)
        {
            _loggingService = loggingService;
            _reagionManager = regionManager;
        }

        // デストラクタ

        // プライベート関数
        private void ExecuteSettingOpenCommand()
        {
            var window = new SettingView()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowMinButton = false,
                ShowMaxRestoreButton = false
            };
            window.Closed += (o, args) => _loggingService.Logs.Add("設定が更新されました。");
            window.Closed += (o, args) => window = null;
            window.ShowDialog();
        }
    }
}
