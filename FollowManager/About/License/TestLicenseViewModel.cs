using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;

namespace FollowManager.About.License
{
    public class TestLicenseViewModel
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

        // コンストラクタ

        /// <summary>
        /// XAMLデザイナー用コンストラクタ
        /// </summary>
        public TestLicenseViewModel()
        {
            var licenseModel = new LicenseModel();
            Licenses.Value = licenseModel.GetLicenses();
            SelectedLicense.Value = Licenses.Value.First();
        }
    }
}
