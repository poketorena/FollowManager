using System.Windows;
using FollowManager.BottomPanel;
using FollowManager.CardPanel;
using FollowManager.SidePanel;
using MahApps.Metro.Controls;
using Prism.Regions;

namespace FollowManager.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : MetroWindow
    {
        public MainWindowView(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("SideRegion", typeof(SidePanelView));
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(CardPanelView));
            regionManager.RegisterViewWithRegion("BottomRegion", typeof(BottomPanelView));
        }
    }
}
