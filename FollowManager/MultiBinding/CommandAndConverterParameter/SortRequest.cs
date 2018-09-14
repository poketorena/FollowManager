using FollowManager.FilterAndSort;
using FollowManager.Tab;

namespace FollowManager.MultiBinding.CommandAndConverterParameter
{
    /// <summary>
    /// タブのデータとソートキータイプ
    /// </summary>
    public class SortRequest
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        public TabData TabData { get; set; }

        /// <summary>
        /// ソートキータイプ
        /// </summary>
        public string SortKeyType { get; set; }
    }
}
