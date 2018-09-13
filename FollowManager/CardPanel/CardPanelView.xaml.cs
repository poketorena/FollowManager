using System.Windows;
using System.Windows.Controls;

namespace FollowManager.CardPanel
{
    /// <summary>
    /// Interaction logic for CardPanelView
    /// </summary>
    public partial class CardPanelView : UserControl
    {
        public CardPanelView()
        {
            InitializeComponent();
        }

        public string TabId
        {
            get { return (string)GetValue(TabIdProperty); }
            set { SetValue(TabIdProperty, value); }
        }

        public static readonly DependencyProperty TabIdProperty =
            DependencyProperty.Register("TabId", typeof(string), typeof(CardPanelView), new PropertyMetadata());
    }
}
