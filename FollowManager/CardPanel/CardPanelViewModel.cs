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

        private DelegateCommand<string> _clickCommand;
        public DelegateCommand<string> ClickCommand =>
            _clickCommand ?? (_clickCommand = new DelegateCommand<string>(ExecuteClickCommand));

        private IDisposable _disposable;

        private void ExecuteClickCommand(string screenName)
        {
            // プロフィールページをブラウザで開く
            var url = "https://twitter.com/" + screenName;
            System.Diagnostics.Process.Start(url);
        }

        // コンストラクタ
        public CardPanelViewModel(AccountManager accountManager)
        {
            _accountManager = accountManager;

            _disposable = _accountManager
                .Current
                .Follows
                .CollectionChangedAsObservable()
                .Subscribe(x =>
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
