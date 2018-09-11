using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dragablz;
using FollowManager.MainWindow;
using Microsoft.Practices.Unity;

namespace FollowManager.Tab
{
    public class MyInterTabClient : IInterTabClient
    {
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var view = new TabHostWindow();
            // 実際はMainWindowViewModelCopyではなくMainWindowViewModelを使う（コンストラクタでタブを生成しないので問題ない！）
            view.DataContext = _unityContainer.Resolve<MainWindowViewModelCopy>();
            return new NewTabHost<TabHostWindow>(view, view.TabsContainer);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window) => throw new NotImplementedException();

        // DI注入される変数

        private readonly IUnityContainer _unityContainer;

        // コンストラクタ
        public MyInterTabClient(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }
    }
}
