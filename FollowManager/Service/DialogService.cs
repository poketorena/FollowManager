using FollowManager.About;
using FollowManager.AddAccount;
using FollowManager.ManageAccount;
using FollowManager.Setting;
using FollowManager.Tab;

namespace FollowManager.Service
{
    public class DialogService
    {
        /// <summary>
        /// 設定画面を開きます。
        /// </summary>
        public void OpenSettingView()
        {
            var window = new SettingView();
            window.Closed += (o, args) => _loggingService.Logs.Add("設定が更新されました。");
            window.Closed += (o, args) => window = null;
            window.ShowDialog();
        }

        /// <summary>
        /// About画面を開きます。
        /// </summary>
        public void OpenAboutView()
        {
            var window = new AboutView();
            window.Closed += (o, args) => _loggingService.Logs.Add("About画面を閉じました。");
            window.Closed += (o, args) => window = null;
            window.ShowDialog();
        }

        /// <summary>
        /// Apiキー設定画面を開きます。
        /// </summary>
        public void OpenConfigureApiKeyView()
        {
            _configureApiKeyView = new ConfigureApiKeyView();
            _configureApiKeyView.Closed += (o, args) => _configureApiKeyView = null;
            _configureApiKeyView.ShowDialog();
        }

        /// <summary>
        /// Apiキー設定画面を閉じます。
        /// </summary>
        public void CloseConfigureApiKeyView()
        {
            _configureApiKeyView.Close();
        }

        /// <summary>
        /// Pinコード設定画面を開きます。
        /// </summary>
        public void OpenConfigurePincodeView()
        {
            _configurePincodeView = new ConfigurePincodeView();
            _configurePincodeView.Closed += (o, args) => _configurePincodeView = null;
            _configurePincodeView.ShowDialog();
        }

        /// <summary>
        /// Pinコード設定画面を閉じます。
        /// </summary>
        public void CloseConfigurePincodeView()
        {
            _configurePincodeView.Close();
        }

        /// <summary>
        /// アカウント管理画面を開きます。
        /// </summary>
        public void OpenManageAccountView()
        {
            var window = new ManageAccountView();
            window.Closed += (o, args) => _loggingService.Logs.Add("アカウント管理画面を閉じました。");
            window.Closed += (o, args) => window = null;
            window.ShowDialog();
        }

        /// <summary>
        /// アカウントタブ追加画面を開きます。
        /// </summary>
        public void OpenAddAccountTabView()
        {
            _addAccountTabView = new AddAccountTabView();
            _addAccountTabView.Closed += (o, args) => _addAccountTabView = null;
            _addAccountTabView.ShowDialog();
        }

        /// <summary>
        /// アカウントタブ追加画面を閉じます。
        /// </summary>
        public void CloseAddAccountTabView()
        {
            _addAccountTabView.Close();
        }

        // プライベート変数

        private ConfigureApiKeyView _configureApiKeyView;

        private ConfigurePincodeView _configurePincodeView;

        private AddAccountTabView _addAccountTabView;

        // DI注入される変数

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public DialogService(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }
    }
}
