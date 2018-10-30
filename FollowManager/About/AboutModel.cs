using System;
using System.Diagnostics;
using FollowManager.Service;

namespace FollowManager.About
{
    public class AboutModel
    {
        // パブリックメソッド

        /// <summary>
        /// ブラウザでUriを開きます。
        /// </summary>
        /// <param name="uri">ブラウザで開くUri</param>
        public void OpenUriCommand(string uri)
        {
            try
            {
                Process.Start(uri);
            }
            catch (Exception)
            {
                const string errorMessage = "ブラウザを開くことに失敗しました。";
                _loggingService.Logs.Add(errorMessage);
                Debug.WriteLine(errorMessage);
            }
        }

        // DI注入されるフィールド

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public AboutModel(LoggingService loggingservice)
        {
            // DI
            _loggingService = loggingservice;
        }
    }
}
