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

        // DI注入される変数
        AccountManager _accountManager;
        CardPanelModel _cardPanelModel;

        private DelegateCommand<string> _openProfileCommand;
        public DelegateCommand<string> OpenProfileCommand =>
            _openProfileCommand ?? (_openProfileCommand = new DelegateCommand<string>(_cardPanelModel.OpenProfile));

        private IDisposable _disposable;

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
        
        ~CardPanelViewModel()
        {
            _disposable.Dispose();
        }
    }
}
