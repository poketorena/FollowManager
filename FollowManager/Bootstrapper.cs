using System.Windows;
using FollowManager.Account;
using FollowManager.CardPanel;
using FollowManager.MainWindow;
using FollowManager.Service;
using FollowManager.SidePanel;
using FollowManager.Tab;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;

namespace FollowManager
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindowView>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // 毎回作る
            Container.RegisterType<CardPanelModel>();
            Container.RegisterType<SidePanelModel>();
            Container.RegisterType<Account.Account>();
            Container.RegisterType<AddAccountTabModel>();
            Container.RegisterType<MainWindowModel>();

            // シングルトンでコンテナに登録
            Container.RegisterType<AccountManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<LoggingService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<DialogService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<AddAccountService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<TabManager>(new ContainerControlledLifetimeManager());
        }
    }
}
