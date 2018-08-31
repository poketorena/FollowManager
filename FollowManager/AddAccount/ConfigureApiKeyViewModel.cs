using FollowManager.Validation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace FollowManager.AddAccount
{
    public class ConfigureApiKeyViewModel
    {
        // パブリックプロパティ

        /// <summary>
        /// コンシューマーキー
        /// </summary>
        [NotEmptyValidation(ErrorMessage = "フィールドConsumer Keyは必須です。")]
        public ReactiveProperty<string> ConsumerKey { get; } = new ReactiveProperty<string>("ConsumerKey");

        /// <summary>
        /// コンシューマーシークレット
        /// </summary>
        [NotEmptyValidation(ErrorMessage = "フィールドConsumer Secretは必須です。")]
        public ReactiveProperty<string> ConsumerSecret { get; } = new ReactiveProperty<string>("ConsumerSecret");

        // パブリック関数

        // デリゲートコマンド

        public ReactiveCommand NextCommand { get; private set; }

        // インタラクションリクエスト

        // イベント

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        // DI注入される変数

        // コンストラクタ

        public ConfigureApiKeyViewModel()
        {
            // バリデーションを有効化する
            ConsumerKey.SetValidateAttribute(() => ConsumerKey);
            ConsumerSecret.SetValidateAttribute(() => ConsumerSecret);

            // ConsumerKeyとConsumerKeyが正しく入力されているときのみボタンを押せるようにする
            NextCommand = new[]
            {
                ConsumerKey.ObserveHasErrors,
                ConsumerSecret.ObserveHasErrors
            }
            .CombineLatestValuesAreAllFalse()
            .ToReactiveCommand()
            .AddTo(Disposables);
        }

        // デストラクタ

        ~ConfigureApiKeyViewModel()
        {
            Disposables.Dispose();
        }

        // プライベート関数
    }
}
