using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FollowManager.Account;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.ManageAccount
{
    public class ManageAccountViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// アカウント
        /// </summary>
        public ReadOnlyReactiveCollection<Account.Account> Accounts
        {
            get { return _accounts; }
            private set { SetProperty(ref _accounts, value); }
        }

        // デリゲートコマンド

        /// <summary>
        /// アカウントを削除するコマンド
        /// </summary>
        public DelegateCommand<Account.Account> DeleteAccountCommand =>
            _deleteAccountCommand ?? (_deleteAccountCommand = new DelegateCommand<Account.Account>(_accountManager.DeleteAccount));

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        private ReadOnlyReactiveCollection<Account.Account> _accounts;

        private DelegateCommand<Account.Account> _deleteAccountCommand;

        // DI注入される変数

        private readonly AccountManager _accountManager;

        // コンストラクタ

        public ManageAccountViewModel(AccountManager accountManager)
        {
            // DI
            _accountManager = accountManager;

            // アカウントを購読して現在登録されているアカウントを更新する
            _accountManager
                .Accounts
                .CollectionChangedAsObservable()
                .Subscribe(_ =>
                {
                    Accounts = _accountManager
                    .Accounts
                    .Values
                    .ToObservable()
                    .ToReadOnlyReactiveCollection();
                })
            .AddTo(Disposables);

            // 最初の1回は手動で代入する
            Accounts = _accountManager
                .Accounts
                .Values
                .ToObservable()
                .ToReadOnlyReactiveCollection();
        }

        // デストラクタ

        ~ManageAccountViewModel()
        {
            Disposables.Dispose();
        }
    }
}
