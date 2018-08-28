using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace FollowManager.Service
{
    public class LoggingService : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// ログを保存するコレクション
        /// </summary>
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();
    }
}
