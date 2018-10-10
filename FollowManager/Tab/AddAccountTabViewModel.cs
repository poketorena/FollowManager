using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FollowManager.Account;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.Tab
{
    public class AddAccountTabViewModel : BindableBase, IDisposable
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

        /// <summary>
        /// 呼び出し元ウィンドウのオブジェクトId
        /// </summary>
        public string CallerObjectId { get; set; }

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
        /// 新しいアカウントタブを作成するコマンド
        /// </summary>
        public DelegateCommand<object[]> AddAccountTabCommand =>
            _addAccountTabCommand ?? (_addAccountTabCommand = new DelegateCommand<object[]>(_addAccountTabModel.AddAccountTab));

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベートフィールド

        private ReadOnlyReactiveCollection<Account.Account> _accounts;

        private DelegateCommand<object[]> _addAccountTabCommand;

        // DI注入されるフィールド

        private readonly AccountManager _accountManager;

        private readonly AddAccountTabModel _addAccountTabModel;

        // コンストラクタ

        public AddAccountTabViewModel(AccountManager accountManager, AddAccountTabModel addAccountTabModel)
        {
            // DI
            _accountManager = accountManager;
            _addAccountTabModel = addAccountTabModel;

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
                .ToReadOnlyReactiveCollection()
                .AddTo(Disposables);
        }
    }
}
