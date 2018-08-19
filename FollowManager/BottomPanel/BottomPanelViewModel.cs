using FollowManager.Service;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.BottomPanel
{
    public class BottomPanelViewModel : BindableBase
    {
        // プロパティ
        public ReactiveProperty<string> Log { get; } = new ReactiveProperty<string>();

        // DI注入される変数
        private readonly LoggingService _loggingService;

        // コンストラクタ
        public BottomPanelViewModel(LoggingService loggingService)
        {
            // DI
            _loggingService = loggingService;

            // プロパティの設定
            Log = _loggingService
                .Logs
                .ObserveAddChanged()
                .ToReactiveProperty();

            // 変更通知のテスト
            _loggingService.Logs.Add("おはようございます");
            _loggingService.Logs.Add("こんにちは");
            _loggingService.Logs.Add("こんばんは");
            _loggingService.Logs.Add("ちゃろー！");
        }
    }
}
