﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FollowManager.Account;
using FollowManager.FilterAndSort;
using FollowManager.Service;
using FollowManager.SidePanel;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FollowManager.CardPanel
{
    public class CardPanelViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのId
        /// </summary>
        public ReactiveProperty<string> TabId { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// 現在表示しているユーザーデータのコレクション
        /// </summary>
        public ReactiveCollection<UserData> UserDatas
        {
            get { return _userDatas; }
            set { SetProperty(ref _userDatas, value); }
        }

        // デリゲートコマンド

        /// <summary>
        /// Twitterのプロフィールページを規定のブラウザで開くコマンド
        /// </summary>
        public DelegateCommand<string> OpenProfileCommand =>
            _openProfileCommand ?? (_openProfileCommand = new DelegateCommand<string>(_cardPanelModel.OpenProfile));

        /// <summary>
        /// お気に入りを切り替えるコマンド
        /// </summary>
        public DelegateCommand<UserData> FavoriteCommnad =>
            _favoriteCommand ?? (_favoriteCommand = new DelegateCommand<UserData>(x =>
            {
                // お気に入りを反転させる
                UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite = !UserDatas.ElementAt(UserDatas.IndexOf(x)).Favorite;
            }));

        /// <summary>
        /// 指定したユーザーをブロックして、3秒後にブロック解除するコマンド
        /// </summary>
        public DelegateCommand<UserData> BlockAndBlockReleaseCommand =>
            _blockAndBlockReleaseCommnad ?? (_blockAndBlockReleaseCommnad = new DelegateCommand<UserData>(async x =>
            {
                UserDatas.ElementAt(UserDatas.IndexOf(x)).FollowType = FollowType.BlockAndBlockRelease;
                await _cardPanelModel.BlockAndBlockReleaseAsync(x.User);
            }));

        // プライベートプロパティ

        /// <summary>
        /// IDisposableのコレクション
        /// </summary>
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // プライベート変数

        private ReactiveCollection<UserData> _userDatas;

        private DelegateCommand<string> _openProfileCommand;

        private DelegateCommand<UserData> _favoriteCommand;

        private DelegateCommand<UserData> _blockAndBlockReleaseCommnad;

        // DI注入される変数

        private readonly AccountManager _accountManager;

        private readonly LoggingService _loggingService;

        private readonly CardPanelModel _cardPanelModel;

        private readonly SidePanelModel _sidePanelModel;

        // コンストラクタ

        public CardPanelViewModel(AccountManager accountManager, LoggingService loggingService, CardPanelModel cardPanelModel, SidePanelModel sidePanelModel)
        {
            // DI
            _accountManager = accountManager;
            _loggingService = loggingService;
            _cardPanelModel = cardPanelModel;
            _sidePanelModel = sidePanelModel;

            // 起動時のロード
            //UserDatas = new ReactiveCollection<UserData>(_accountManager.Current.Followers.Take(20).ToObservable());

            // ロード完了時に発生するイベント購読して現在表示しているユーザーデータのコレクションを更新する
            Observable.FromEvent<IEnumerable<UserData>>(
                handler => _cardPanelModel.LoadCompleted += handler,
                handler => _cardPanelModel.LoadCompleted -= handler
                )
                .Subscribe(userData =>
                {
                    UserDatas = (userData ?? new List<UserData>())
                    .ToObservable()
                    .ToReactiveCollection();
                })
                .AddTo(Disposables);

            // ソートキー、ソート順を購読して現在表示しているユーザーデータのコレクションをソートする
            //_sidePanelModel
            //    .FilterAndSortOption
            //    .PropertyChangedAsObservable()
            //    .Where(args => args.PropertyName == nameof(SortKeyType) || args.PropertyName == nameof(SortOrderType))
            //    .Subscribe(_ => Sort())
            //    .AddTo(Disposables);

            // タブのIdをCardPanelModelに書き戻す
            TabId
                .PropertyChangedAsObservable()
                .Subscribe(_ =>
                {
                    _cardPanelModel.TabId = TabId.Value;
                });
        }

        // デストラクタ

        ~CardPanelViewModel()
        {
            Disposables.Dispose();
        }
    }
}
