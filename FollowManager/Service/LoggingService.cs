using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace FollowManager.Service
{
    public class LoggingService : BindableBase
    {
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();
    }
}
