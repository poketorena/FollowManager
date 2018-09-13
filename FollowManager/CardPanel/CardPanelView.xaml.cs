using System.Windows;
using System.Windows.Controls;
using FollowManager.Tab;

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

        public TabData TabData
        {
            get { return (TabData)GetValue(TabDataProperty); }
            set { SetValue(TabDataProperty, value); }
        }

        public static readonly DependencyProperty TabDataProperty =
            DependencyProperty.Register("TabData", typeof(TabData), typeof(CardPanelView), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTabDataChanged)));

        /// <summary>
        /// タブのデータが変更されたときに呼び出され、タブのIdをCardPanelViewModelに伝えます。
        /// </summary>
        /// <param name="dependencyObject">CardPanelView</param>
        /// <param name="dependencyPropertyChangedEventArgs">未使用</param>
        private static void OnTabDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is CardPanelView cardPanelView)
            {
                if (cardPanelView.DataContext is CardPanelViewModel cardPanelViewModel)
                {
                    cardPanelViewModel.TabId.Value = cardPanelView.TabData.TabId;
                }
            }
        }
    }
}
