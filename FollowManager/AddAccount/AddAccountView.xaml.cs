using MahApps.Metro.Controls;
using Prism.Regions;

namespace FollowManager.AddAccount
{
    /// <summary>
    /// Interaction logic for AddAccountView.xaml
    /// </summary>
    public partial class AddAccountView : MetroWindow
    {
        public AddAccountView(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("AddAccountRegion", typeof(ConfigureApiKeyView));
        }
    }
}
