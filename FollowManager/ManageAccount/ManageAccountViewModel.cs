using System.Reactive.Disposables;
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
        public ReadOnlyReactiveCollection<Account.Account> Accounts { get; }

        // デリゲートコマンド

        /// <summary>
        /// アカウントを削除するコマンド
        /// </summary>
        public DelegateCommand<Account.Account> DeleteAccountCommand  =>
            _deleteAccountCommand ?? (_deleteAccountCommand = new DelegateCommand<Account.Account>(_accountManager.DeleteAccount));

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        private DelegateCommand<Account.Account> _deleteAccountCommand;

        // DI注入される変数

        private readonly AccountManager _accountManager;

        // コンストラクタ

        public ManageAccountViewModel(AccountManager accountManager)
        {
            // DI
            _accountManager = accountManager;

            // アカウントの監視
            Accounts = _accountManager
                .Accounts
                .ToReadOnlyReactiveCollection()
                .AddTo(Disposables);
        }

        // デストラクタ

        ~ManageAccountViewModel()
        {
            Disposables.Dispose();
        }
    }
}
