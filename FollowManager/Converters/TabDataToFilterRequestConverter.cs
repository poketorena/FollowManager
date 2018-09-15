using System;
using System.Globalization;
using System.Windows.Data;
using FollowManager.MultiBinding.CommandAndConverterParameter;
using FollowManager.Tab;

namespace FollowManager.Converters
{
    /// <summary>
    /// TabDataをFilterRequest型に変換するコンバーター
    /// </summary>
    public class TabDataToFilterRequestConverter : IValueConverter
    {
        /// <summary>
        /// TabDataをFilterRequest型に変換します。
        /// </summary>
        /// <param name="value">タブのデータ</param>
        /// <param name="targetType">未使用</param>
        /// <param name="parameter">Viewで指定するFilterTypeの文字列</param>
        /// <param name="culture">未使用</param>
        /// <returns>タブのデータとフィルタの種類</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TabData tabData && parameter is string filterType)
            {
                return new FilterRequest { TabData = tabData, FilterType = filterType };
            }
            else
            {
                return new FilterRequest();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
