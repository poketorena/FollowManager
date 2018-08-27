using FollowManager.About;
using FollowManager.Account;
using FollowManager.Service;
using FollowManager.Setting;
using Prism.Commands;
using Prism.Mvvm;

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
        readonly AccountManager _accountManager;
        readonly LoggingService _loggingService;

        // コンストラクタ
        public MainWindowViewModel(AccountManager accountManager, LoggingService loggingService)
        {
            _accountManager = accountManager;
            _loggingService = loggingService;
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
