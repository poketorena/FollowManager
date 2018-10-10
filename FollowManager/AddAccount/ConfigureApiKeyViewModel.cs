using System;
using System.Reactive.Disposables;
using FollowManager.Api;
using FollowManager.Service;
using FollowManager.Validation;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.AddAccount
{
    public class ConfigureApiKeyViewModel : BindableBase, IDisposable
    {
        // パブリックプロパティ

        /// <summary>
        /// コンシューマーキー
        /// </summary>
        [NotEmptyValidation(ErrorMessage = "フィールドConsumer Keyは必須です。")]
        public ReactiveProperty<string> ConsumerKey { get; } = new ReactiveProperty<string>(TwitterApiKey.ConsumerKey);

        /// <summary>
        /// コンシューマーシークレット
        /// </summary>
        [NotEmptyValidation(ErrorMessage = "フィールドConsumer Secretは必須です。")]
        public ReactiveProperty<string> ConsumerSecret { get; } = new ReactiveProperty<string>(TwitterApiKey.ConsumerSecret);

        // パブリックメソッド

        /// <summary>
        /// リソースを破棄します。
        /// </summary>
        public void Dispose()
        {
            Disposables.Dispose();
        }

        // コマンド

        /// <summary>
        /// Pinコード設定画面を開くコマンド
        /// </summary>
        public ReactiveCommand NextCommand { get; }

        /// <summary>
        /// Apiキー設定画面を閉じるコマンド
        /// </summary>
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(_dialogService.CloseConfigureApiKeyView));

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベートフィールド

        private DelegateCommand _cancelCommand;

        // DI注入されるフィールド

        private readonly DialogService _dialogService;

        private readonly AddAccountService _addAccountService;

        // コンストラクタ

        public ConfigureApiKeyViewModel(DialogService dialogService, AddAccountService addAccountModel)
        {
            // DI
            _dialogService = dialogService;
            _addAccountService = addAccountModel;

            // バリデーションを有効化する
            ConsumerKey.SetValidateAttribute(() => ConsumerKey);
            ConsumerSecret.SetValidateAttribute(() => ConsumerSecret);

            // ConsumerKeyとConsumerKeyが正しく入力されているときのみ「次へ」を押せるようにする
            NextCommand = new[]
            {
                ConsumerKey.ObserveHasErrors,
                ConsumerSecret.ObserveHasErrors
            }
            .CombineLatestValuesAreAllFalse()
            .ToReactiveCommand()
            .AddTo(Disposables);

            // 「次へ」が押されたらPinコード設定画面を開く
            NextCommand
                .Subscribe(() =>
                {
                    _addAccountService.OpenAuthorizeUrl(ConsumerKey.Value, ConsumerSecret.Value);
                    _dialogService.OpenConfigurePincodeView();
                })
                .AddTo(Disposables);
        }
    }
}
