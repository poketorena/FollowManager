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
using FollowManager.FilterAndSort;
using FollowManager.Service;
using Prism.Regions;
using System.Diagnostics;

namespace FollowManager.CardPanel
{
    public class CardPanelViewModel : BindableBase
    {
        // プロパティ
        private ReactiveCollection<UserData> _userDatas;
        public ReactiveCollection<UserData> UserDatas
        {
            get { return _userDatas; }
            set { SetProperty(ref _userDatas, value); }
        }

        // パブリック関数

        // デリゲートコマンド
        private DelegateCommand<string> _openProfileCommand;
        public DelegateCommand<string> OpenProfileCommand =>
            _openProfileCommand ?? (_openProfileCommand = new DelegateCommand<string>(_cardPanelModel.OpenProfile));

        private DelegateCommand<UserData> _favoriteCommand;
        public DelegateCommand<UserData> FavoriteCommnad =>
            _favoriteCommand ?? (_favoriteCommand = new DelegateCommand<UserData>(x =>
            {
                // Favoriteを反転させる
                UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite = !UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite;
            }));

        private DelegateCommand<UserData> _blockAndBlockReleaseCommnad;
        public DelegateCommand<UserData> BlockAndBlockReleaseCommand =>
            _blockAndBlockReleaseCommnad ?? (_blockAndBlockReleaseCommnad = new DelegateCommand<UserData>(async x =>
            {
                UserDatas.ElementAt(UserDatas.IndexOf(x)).FollowType = FollowType.BlockAndBlockRelease;
                await _cardPanelModel.BlockAndBlockReleaseAsync(x.User);
            }));


        // インタラクションリクエスト

        // プライベート変数
        private IDisposable _filter;

        // DI注入される変数
        private readonly AccountManager _accountManager;
        private readonly CardPanelModel _cardPanelModel;
        private readonly LoggingService _loggingService;

        // コンストラクタ
        public CardPanelViewModel(AccountManager accountManager, CardPanelModel cardPanelModel, LoggingService loggingService)
        {
            _accountManager = accountManager;
            _cardPanelModel = cardPanelModel;
            _loggingService = loggingService;

            //UserDatas = new ReactiveCollection<UserData>(_accountManager.Current.Followers.Take(20).ToObservable());

            _filter = Observable.FromEvent<List<UserData>>(
                handler => _cardPanelModel.LoadCompleted += handler,
                handler => _cardPanelModel.LoadCompleted -= handler
                )
                .Subscribe(userData =>
                {
                    UserDatas = (userData ?? new List<UserData>())
                    .ToObservable()
                    .ToReactiveCollection();
                });
        }

        // デストラクタ
        ~CardPanelViewModel()
        {
            _filter.Dispose();
        }

        // プライベート関数
    }
}
