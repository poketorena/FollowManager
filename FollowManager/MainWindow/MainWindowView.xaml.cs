using System;
using System.Linq;
using System.Windows;
using FollowManager.BottomPanel;
using FollowManager.CardPanel;
using FollowManager.Dispose;
using FollowManager.SidePanel;
using MahApps.Metro.Controls;
using Prism.Regions;
using Reactive.Bindings.Extensions;

namespace FollowManager.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : MetroWindow
    {
        public MainWindowView()
        {
            InitializeComponent();

            // アカウントタブ追加時にタブが選択された状態になるようにする
            ((MainWindowViewModel)DataContext)
                .TabDatas
                .Value
                .ObserveAddChangedItems()
                .Subscribe(addTabDatas => tabablzControl.SelectedItem = addTabDatas.Last())
                .AddTo(DisposeManager.Instance.Disposables);
        }
    }
}
