using FollowManager.MainWindow;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using FollowManager.Service;
using FollowManager.Account;
using FollowManager.CardPanel;

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

            Container.RegisterType<CardPanelModel>();

            // シングルトンでコンテナに登録
            Container.RegisterType<LoggingService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<AccountManager>(new ContainerControlledLifetimeManager());
        }
    }
}
