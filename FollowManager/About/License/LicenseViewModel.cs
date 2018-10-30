using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;

namespace FollowManager.About.License
{
    public class LicenseViewModel : BindableBase
    {
        // パブリックプロパティ

        /// <summary>
        /// ライセンスのコレクション
        /// </summary>
        public ReactiveProperty<IEnumerable<Tuple<string, string, string>>> Licenses { get; } = new ReactiveProperty<IEnumerable<Tuple<string, string, string>>>();

        /// <summary>
        /// 選択されたライセンス
        /// </summary>
        public ReactiveProperty<Tuple<string, string, string>> SelectedLicense { get; } = new ReactiveProperty<Tuple<string, string, string>>();

        // コマンド

        public DelegateCommand<object[]> ChangeSelectedLicenseCommand =>
            _changeSelectedLicenseCommand ?? (_changeSelectedLicenseCommand = new DelegateCommand<object[]>(selectedLicense => SelectedLicense.Value = (Tuple<string, string, string>)selectedLicense[0]));

        // プライベートフィールド

        private DelegateCommand<object[]> _changeSelectedLicenseCommand;

        // DI注入されるフィールド

        private readonly LicenseModel _licenseModel;

        // コンストラクタ

        public LicenseViewModel(LicenseModel licenseModel)
        {
            // DI
            _licenseModel = licenseModel;

            // ライセンスを初期化
            Licenses.Value = _licenseModel.GetLicenses();

            // 先頭のライセンスを選択された状態にする
            SelectedLicense.Value = Licenses.Value.First();
        }
    }
}
