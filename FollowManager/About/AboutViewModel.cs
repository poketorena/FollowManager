using Prism.Commands;
using Prism.Mvvm;
using Unity.Attributes;

namespace FollowManager.About
{
    public class AboutViewModel : BindableBase
    {
        // コマンド

        /// <summary>
        /// ブラウザでUriを開くコマンド
        /// </summary>
        public DelegateCommand<string> OpenUriCommand =>
            _openUriCommand ?? (_openUriCommand = new DelegateCommand<string>(_aboutModel.OpenUriCommand));

        // プライベートフィールド

        private DelegateCommand<string> _openUriCommand;

        // DI注入されるフィールド

        private readonly AboutModel _aboutModel;

        // コンストラクタ

        /// <summary>
        /// XAMLデザイナー用コンストラクタ
        /// </summary>
        public AboutViewModel() { }

        [InjectionConstructor]
        public AboutViewModel(AboutModel aboutModel)
        {
            _aboutModel = aboutModel;
        }
    }
}