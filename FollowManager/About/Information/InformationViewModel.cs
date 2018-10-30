using Prism.Mvvm;
using Reactive.Bindings;

namespace FollowManager.About.Information
{
    public class InformationViewModel : BindableBase
    {
        // パブリックプロパティ

        public ReactiveProperty<string> Version { get; } = new ReactiveProperty<string>();

        // DI注入されるフィールド

        private readonly InformationModel _informationModel;

        // コンストラクタ

        public InformationViewModel(InformationModel informationModel)
        {
            // DI
            _informationModel = informationModel;

            Version.Value = _informationModel.GetVersion();
        }
    }
}
