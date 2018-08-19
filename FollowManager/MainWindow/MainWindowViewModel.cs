using Prism.Mvvm;

namespace FollowManager.MainWindow
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Follow Manager";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
