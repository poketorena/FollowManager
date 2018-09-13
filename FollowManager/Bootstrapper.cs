using FollowManager.MainWindow;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using FollowManager.Service;
using FollowManager.Account;
using FollowManager.CardPanel;
using FollowManager.SidePanel;
using FollowManager.AddAccount;
using FollowManager.Tab;

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

            // シングルトンでコンテナに登録
            Container.RegisterType<AccountManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<LoggingService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<DialogService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<AddAccountService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<TabManager>(new ContainerControlledLifetimeManager());
        }
    }
}
