using System.Windows;
using FollowManager.About;
using FollowManager.About.Information;
using FollowManager.About.License;
using FollowManager.Account;
using FollowManager.CardPanel;
using FollowManager.Dispose;
using FollowManager.MainWindow;
using FollowManager.Service;
using FollowManager.SidePanel;
using FollowManager.Tab;
using Prism.Ioc;
using Prism.Unity;

namespace FollowManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DisposeManager.Instance.Disposables.Dispose();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindowView>();
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 毎回作る
            containerRegistry.Register<CardPanelModel>();
            containerRegistry.Register<SidePanelModel>();
            containerRegistry.Register<Account.Account>();
            containerRegistry.Register<AddAccountTabModel>();
            containerRegistry.Register<InformationModel>();
            containerRegistry.Register<LicenseModel>();
            containerRegistry.Register<AboutModel>();

            // シングルトンでコンテナに登録
            containerRegistry.RegisterSingleton<AccountManager>();
            containerRegistry.RegisterSingleton<LoggingService>();
            containerRegistry.RegisterSingleton<DialogService>();
            containerRegistry.RegisterSingleton<AddAccountService>();
            containerRegistry.RegisterSingleton<TabManager>();
            containerRegistry.RegisterSingleton<ApplicationService>();
        }
    }
}
