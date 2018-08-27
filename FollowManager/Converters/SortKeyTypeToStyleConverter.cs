using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FollowManager.FilterAndSort;

namespace FollowManager.Converters
{
    /// <summary>
    /// SortKeyType型からStyle型に変換するコンバーター。現在のソートキーを強調表示するために動的にスタイルを変更します。
    /// </summary>
    public class SortKeyTypeToStyleConverter : IMultiValueConverter
    {
        /// <summary>
        /// SortKeyType型からStyle型に変換します。
        /// </summary>
        /// <param name="values">values[0]がスタイルを適応するボタン、values[1]がViewModelのSortKeyTypeプロパティ</param>
        /// <param name="targetType">未使用</param>
        /// <param name="parameter">Viewで指定するSortKeyTypeの文字列</param>
        /// <param name="culture">未使用</param>
        /// <returns>変更後のボタンのスタイル</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var targetElement = values[0] as FrameworkElement;

            var sortKeyType = (SortKeyType)values[1];

            var buttonType = (SortKeyType)Enum.Parse(typeof(SortKeyType), (string)parameter);

            const string flatButtonStyleName = "MaterialDesignFlatButton";

            const string raisedButtonStyleName = "MaterialDesignRaisedButton";

            if (buttonType == sortKeyType)
            {
                return (Style)targetElement.TryFindResource(raisedButtonStyleName);
            }
            else
            {
                return (Style)targetElement.TryFindResource(flatButtonStyleName);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
