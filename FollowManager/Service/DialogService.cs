using FollowManager.About;
using FollowManager.Setting;

namespace FollowManager.Service
{
    public class DialogService
    {
        // パブリック関数

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

        // DI注入される変数

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public DialogService(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }
    }
}
