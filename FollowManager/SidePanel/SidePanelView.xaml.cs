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
            DependencyProperty.Register("TabData", typeof(TabData), typeof(SidePanelView), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTabDataChanged)));

        /// <summary>
        /// タブのデータが変更されたときに呼び出され、タブのIdをSidePanelViewModelに伝えます。
        /// </summary>
        /// <param name="dependencyObject">SidePanelView</param>
        /// <param name="dependencyPropertyChangedEventArgs">未使用</param>
        private static void OnTabDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is SidePanelView sidePanelView)
            {
                if (sidePanelView.DataContext is SidePanelViewModel sidePanelViewModel)
                {
                    sidePanelViewModel.TabId.Value = sidePanelView.TabData.TabId;
                }
            }
        }
    }
}
