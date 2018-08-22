using CoreTweet;
using FollowManager.Account;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;

namespace FollowManager.CardPanel
{
    public class CardPanelViewModel : BindableBase
    {
        // プロパティ
        public ReactiveCollection<UserData> Follows { get; set; }

        // パブリック関数

        // DI注入される変数
        readonly AccountManager _accountManager;
        readonly CardPanelModel _cardPanelModel;

        // デリゲートコマンド
        private DelegateCommand<string> _openProfileCommand;
        public DelegateCommand<string> OpenProfileCommand =>
            _openProfileCommand ?? (_openProfileCommand = new DelegateCommand<string>(_cardPanelModel.OpenProfile));

        // インタラクションリクエスト

        // プライベート変数
        private IDisposable _disposable;

        // DI注入される変数

        // コンストラクタ
        public CardPanelViewModel(AccountManager accountManager, CardPanelModel cardPanelModel)
        {
            _accountManager = accountManager;
            _cardPanelModel = cardPanelModel;

            _disposable = _accountManager
                .Current
                .Follows
                .CollectionChangedAsObservable()
                .Subscribe(_ =>
                {
                    Follows = new ReactiveCollection<UserData>(_accountManager.Current.Follows.ToObservable());
                }
                );

            Follows = new ReactiveCollection<UserData>(_accountManager.Current.Follows.ToObservable());
        }

        // デストラクタ
        ~CardPanelViewModel()
        {
            _disposable.Dispose();
        }

        // プライベート関数
    }
}
