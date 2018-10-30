using Reactive.Bindings;

namespace FollowManager.About.Information
{
    public class TestInformationViewModel
    {
        // パブリックプロパティ

        public ReactiveProperty<string> Version { get; } = new ReactiveProperty<string>();

        // コンストラクタ

        /// <summary>
        /// XAMLデザイナー用コンストラクタ
        /// </summary>
        public TestInformationViewModel()
        {
            Version.Value = "1.0.0";
        }
    }
}
