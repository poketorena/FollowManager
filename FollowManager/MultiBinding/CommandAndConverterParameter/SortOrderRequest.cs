using FollowManager.FilterAndSort;
using FollowManager.Tab;

namespace FollowManager.MultiBinding.CommandAndConverterParameter
{
    /// <summary>
    /// タブのデータとソート順の種類
    /// </summary>
    public class SortOrderRequest
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        public TabData TabData { get; set; }

        /// <summary>
        /// ソート順の種類
        /// </summary>
        public SortOrderType SortOrderType { get; set; }
    }
}
