using FollowManager.Tab;

namespace FollowManager.MultiBinding.CommandAndConverterParameter
{
    /// <summary>
    /// フィルタを適応するタブのデータとフィルタタイプ
    /// </summary>
    public class FilterRequest
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        public TabData TabData { get; set; }

        /// <summary>
        /// フィルタタイプ
        /// </summary>
        public string FilterType { get; set; }
    }
}
