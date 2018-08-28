using System.Reactive.Disposables;
using FollowManager.Service;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.BottomPanel
{
    public class BottomPanelViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// 最新のログ
        /// </summary>
        public ReactiveProperty<string> Log { get; }

        // プライベートプロパティ

        /// <summary>
        /// IDisposableのコレクション
        /// </summary>
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // DI注入される変数

        private readonly LoggingService _loggingService;

        // コンストラクタ

        public BottomPanelViewModel(LoggingService loggingService)
        {
            // DI
            _loggingService = loggingService;

            // ログを保存するコレクションの変更を購読して最新のログを更新する
            Log = _loggingService
                .Logs
                .ObserveAddChanged()
                .ToReactiveProperty()
                .AddTo(Disposables);

            // 変更を購読できているかのテスト
            _loggingService.Logs.Add("おはようございます");
            _loggingService.Logs.Add("こんにちは");
            _loggingService.Logs.Add("こんばんは");
            _loggingService.Logs.Add("ちゃろー！");
        }

        // デストラクタ

        ~BottomPanelViewModel()
        {
            Disposables.Dispose();
        }
    }
}
