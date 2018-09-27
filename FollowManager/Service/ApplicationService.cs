using System.Diagnostics;
using System.Windows;

namespace FollowManager.Service
{
    public class ApplicationService
    {
        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// アプリケーションを再起動します。
        /// </summary>
        public void RestartApplication()
        {
            Application.Current.Shutdown();
            Process.Start(Application.ResourceAssembly.Location);
        }
    }
}
