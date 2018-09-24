using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FollowManager.Service;
using FollowManager.Validation;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.AddAccount
{
    public class ConfigurePincodeViewModel : BindableBase, IDisposable
    {
        // パブリックプロパティ

        /// <summary>
        /// Pinコード
        /// </summary>
        [NotEmptyValidation(ErrorMessage = "フィールドPincodeは必須です。")]
        public ReactiveProperty<string> Pincode { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// Pinコードによる認証を開始するコマンド
        /// </summary>
        public ReactiveCommand NextCommand { get; }

        /// <summary>
        /// Pinコード設定画面を閉じるコマンド
        /// </summary>
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(_dialogService.CloseConfigurePincodeView));

        /// <summary>
        /// リソースを破棄します。
        /// </summary>
        public void Dispose()
        {
            Disposables.Dispose();
        }

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        private DelegateCommand _cancelCommand;

        // DI注入される変数

        private readonly DialogService _dialogService;

        private readonly AddAccountService _addAccountService;

        // コンストラクタ

        public ConfigurePincodeViewModel(DialogService dialogService, AddAccountService addAccountModel)
        {
            // DI
            _dialogService = dialogService;
            _addAccountService = addAccountModel;

            // バリデーションを有効化する
            Pincode.SetValidateAttribute(() => Pincode);

            // Pincodeが入力されているときのみ「次へ」を押せるようにする
            NextCommand = Pincode
                .ObserveHasErrors
                .Select(x => !x)
                .ToReactiveCommand()
                .AddTo(Disposables);

            // 「次へ」が押されたらPinコードの認証を開始する
            NextCommand.Subscribe(() =>
            {
                _addAccountService.ConfigureAccessTokens(Pincode.Value);
                _dialogService.CloseConfigurePincodeView();
                _dialogService.CloseConfigureApiKeyView();
            })
            .AddTo(Disposables);
        }
    }
}
