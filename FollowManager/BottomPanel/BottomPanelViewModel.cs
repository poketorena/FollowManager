using FollowManager.Service;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.BottomPanel
{
    public class BottomPanelViewModel : BindableBase
    {
        // DI注入される変数
        private readonly LoggingService _loggingService;

        // プロパティ
        public ReactiveProperty<string> Log { get; } = new ReactiveProperty<string>();

        // コンストラクタ
        public BottomPanelViewModel(LoggingService loggingService)
        {
            _loggingService = loggingService;

            Log = _loggingService
                .Logs
                .ObserveAddChanged()
                .ToReactiveProperty();

            _loggingService.Logs.Add("おはようございます");
            _loggingService.Logs.Add("こんにちは");
            _loggingService.Logs.Add("こんばんは");
            _loggingService.Logs.Add("ちゃろー！");
        }
    }
}
