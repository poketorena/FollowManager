using FollowManager.FilterAndSort;
using FollowManager.Tab;

namespace FollowManager.MultiBinding.CommandAndConverterParameter
{
    /// <summary>
    /// タブのデータとフィルタの種類
    /// </summary>
    public class FilterRequest
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        public TabData TabData { get; set; }

        /// <summary>
        /// フィルタの種類
        /// </summary>
        public FilterType FilterType { get; set; }
    }
}
