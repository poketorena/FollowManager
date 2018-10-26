using System;
using System.Diagnostics;
using FollowManager.Service;

namespace FollowManager.About.Information
{
    public class InformationModel
    {
        // パブリックメソッド

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

        public InformationModel(LoggingService loggingservice)
        {
            _loggingService = loggingservice;
        }
    }
}
