using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;

namespace FollowManager.About.Information
{
    public class InformationViewModel : BindableBase
	{
        // コマンド

        public DelegateCommand<string> OpenUriCommand =>
            _openUriCommand ?? (_openUriCommand = new DelegateCommand<string>(_informationModel.OpenUriCommand));

        // プライベートフィールド

        private DelegateCommand<string> _openUriCommand;

        // DI注入されるフィールド

        private readonly InformationModel _informationModel;

        // コンストラクタ

        public InformationViewModel()
        {

        }

        [InjectionConstructor]
        public InformationViewModel(InformationModel informationModel)
        {
            _informationModel = informationModel;
        }
    }
}
