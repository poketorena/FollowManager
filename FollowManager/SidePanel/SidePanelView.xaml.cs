using System.Windows;
using System.Windows.Controls;
using FollowManager.Tab;

namespace FollowManager.SidePanel
{
    /// <summary>
    /// Interaction logic for SidePanelView
    /// </summary>
    public partial class SidePanelView : UserControl
    {
        public SidePanelView()
        {
            InitializeComponent();
        }

        public TabData TabData
        {
            get { return (TabData)GetValue(TabDataProperty); }
            set { SetValue(TabDataProperty, value); }
        }

        public static readonly DependencyProperty TabDataProperty =
            DependencyProperty.Register("TabData", typeof(TabData), typeof(SidePanelView), new UIPropertyMetadata());
    }
}
