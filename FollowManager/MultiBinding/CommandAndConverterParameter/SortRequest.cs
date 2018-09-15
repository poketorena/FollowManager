using FollowManager.Tab;

namespace FollowManager.MultiBinding.CommandAndConverterParameter
{
    /// <summary>
    /// タブのデータとソートキーの種類
    /// </summary>
    public class SortKeyRequest
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        public TabData TabData { get; set; }

        /// <summary>
        /// ソートキーの種類
        /// </summary>
        public string SortKeyType { get; set; }
    }
}
