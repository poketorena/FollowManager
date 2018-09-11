﻿using System.Reactive.Disposables;
using FollowManager.Account;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.Tab
{
    public class AddAccountTabViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// アカウント
        /// </summary>
        public ReadOnlyReactiveCollection<Account.Account> Accounts { get; }

        /// <summary>
        /// 呼び出し元ウィンドウのオブジェクトId
        /// </summary>
        public string CallerObjectId { get; set; }

        // デリゲートコマンド

        /// <summary>
        /// 新しいアカウントタブを作成するコマンド
        /// </summary>
        public DelegateCommand<object[]> AddAccountTabCommand =>
            _addAccountTabCommand ?? (_addAccountTabCommand = new DelegateCommand<object[]>(_addAccountTabModel.AddAccountTab));

        // プライベートプロパティ

        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        private DelegateCommand<object[]> _addAccountTabCommand;

        // DI注入される変数

        private readonly AccountManager _accountManager;

        private readonly AddAccountTabModel _addAccountTabModel;

        // コンストラクタ

        public AddAccountTabViewModel(AccountManager accountManager, AddAccountTabModel addAccountTabModel)
        {
            // DI
            _accountManager = accountManager;
            _addAccountTabModel = addAccountTabModel;

            // アカウントを購読して現在登録されているアカウントを更新する
            Accounts = _accountManager
                .Accounts
                .ToReadOnlyReactiveCollection()
                .AddTo(Disposables);
        }

        // デストラクタ

        ~AddAccountTabViewModel()
        {
            Disposables.Dispose();
        }
    }
}
