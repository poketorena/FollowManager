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
            _settingView = new SettingView();
            _settingView.Closed += (o, args) =>
            {
                _loggingService.Logs.Add("設定が更新されました。");
                ((SettingViewModel)_settingView.DataContext).Dispose();
                _settingView = null;
            };
            _settingView.ShowDialog();
        }

        /// <summary>
        /// 設定画面を閉じます。
        /// </summary>
        public void CloseSettingView()
        {
            _settingView.Close();
        }

        /// <summary>
        /// About画面を開きます。
        /// </summary>
        public void OpenAboutView()
        {
            _aboutView = new AboutView();
            _aboutView.Closed += (o, args) =>
            {
                _loggingService.Logs.Add("About画面を閉じました。");
                ((AboutViewModel)_aboutView.DataContext).Dispose();
                _aboutView = null;
            };
            _aboutView.ShowDialog();
        }

        /// <summary>
        /// About画面を閉じます。
        /// </summary>
        public void CloseAboutView()
        {
            _aboutView.Close();
        }

        /// <summary>
        /// Apiキー設定画面を開きます。
        /// </summary>
        public void OpenConfigureApiKeyView()
        {
            _configureApiKeyView = new ConfigureApiKeyView();
            _configureApiKeyView.Closed += (o, args) =>
            {
                ((ConfigureApiKeyViewModel)_configureApiKeyView.DataContext).Dispose();
                _configureApiKeyView = null;
            };
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
            _configurePincodeView.Closed += (o, args) =>
            {
                ((ConfigurePincodeViewModel)_configurePincodeView.DataContext).Dispose();
                _configurePincodeView = null;
            };
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
            _manageAccountView = new ManageAccountView();
            _manageAccountView.Closed += (o, args) =>
            {
                ((ManageAccountViewModel)_manageAccountView.DataContext).Dispose();
                _manageAccountView = null;
            };
            _manageAccountView.ShowDialog();
        }

        /// <summary>
        /// アカウントタブ追加画面を開きます。
        /// </summary>
        public void OpenAddAccountTabView()
        {
            _addAccountTabView = new AddAccountTabView();
            _addAccountTabView.Closed += (o, args) =>
            {
                ((AddAccountTabViewModel)_addAccountTabView.DataContext).Dispose();
                _addAccountTabView = null;
            };
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

        private SettingView _settingView;

        private AboutView _aboutView;

        private ConfigureApiKeyView _configureApiKeyView;

        private ConfigurePincodeView _configurePincodeView;

        private ManageAccountView _manageAccountView;

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
