using System;
using System.Globalization;
using System.Windows.Data;
using FollowManager.Account;
using FollowManager.MultiBinding.MultiParameter;
using FollowManager.Tab;

namespace FollowManager.Converters
{
    /// <summary>
    /// UserData型からBlockAndBlockReleaseRequest型に変換するコンバーター。
    /// </summary>
    public class UserDataToBlockAndBlockReleaseRequestConverter : IMultiValueConverter
    {
        /// <summary>
        /// UserData型からBlockAndBlockRelease型に変換します。
        /// </summary>
        /// <param name="values">values[0]がユーザーデータ、values[1]がタブのデータ</param>
        /// <param name="targetType">未使用</param>
        /// <param name="parameter">未使用</param>
        /// <param name="culture">未使用</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is UserData userData && values[1] is TabData tabData)
            {
                return new BlockAndBlockReleaseRequest { TabData = tabData, UserData = userData };
            }
            else
            {
                return new BlockAndBlockReleaseRequest();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
