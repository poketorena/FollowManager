using System;
using System.Globalization;
using System.Windows.Data;
using FollowManager.FilterAndSort;
using FollowManager.MultiBinding.CommandAndConverterParameter;
using FollowManager.Tab;

namespace FollowManager.Converters
{
    /// <summary>
    /// TabData型をSortOrderRequest型に変換するコンバーター
    /// </summary>
    public class TabDataToSortOrderRequestConverter : IValueConverter
    {
        /// <summary>
        /// TabData型をSortOrderRequest型に変換します。
        /// </summary>
        /// <param name="value">タブのデータ</param>
        /// <param name="targetType">未使用</param>
        /// <param name="parameter">Viewで指定するSortOrderの文字列</param>
        /// <param name="culture">未使用</param>
        /// <returns>タブのデータとソート順の種類</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TabData tabData && parameter is string sortOrderType)
            {
                return new SortOrderRequest
                {
                    TabData = tabData,
                    SortOrderType = (SortOrderType)Enum.Parse(typeof(SortOrderType), sortOrderType)
                };
            }
            else
            {
                return new SortOrderRequest();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
