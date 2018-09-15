using System;
using System.Globalization;
using System.Windows.Data;
using FollowManager.FilterAndSort;
using FollowManager.MultiBinding.CommandAndConverterParameter;
using FollowManager.Tab;

namespace FollowManager.Converters
{
    /// <summary>
    /// TabDataをSortKeyRequest型に変換するコンバーター
    /// </summary>
    public class TabDataToSortKeyRequestConverter : IValueConverter
    {
        /// <summary>
        /// TabDataをSortKeyRequest型に変換します。
        /// </summary>
        /// <param name="value">タブのデータ</param>
        /// <param name="targetType">未使用</param>
        /// <param name="parameter">Viewで指定するSortKeyTypeの文字列</param>
        /// <param name="culture">未使用</param>
        /// <returns>タブのデータとソートキーの種類</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TabData tabData && parameter is string sortKeyType)
            {
                return new SortKeyRequest
                {
                    TabData = tabData,
                    SortKeyType = (SortKeyType)Enum.Parse(typeof(SortKeyType), sortKeyType)
                };
            }
            else
            {
                return new SortKeyRequest();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
