using System;
using System.Globalization;
using System.Windows.Data;
using FollowManager.Account;

namespace FollowManager.Converters
{
    /// <summary>
    /// FollowType型からstring型に変換するコンバーター。フォロー関係を表示するバッジの文字列を変更する際に使います。
    /// </summary>
    public class FollowTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(FollowType))
            {
                throw new ArgumentException();
            }

            switch ((FollowType)value)
            {
                case FollowType.OneWay:
                    {
                        return "片思い";
                    }
                case FollowType.Fan:
                    {
                        return "ファン";
                    }
                case FollowType.Mutual:
                    {
                        return "相互フォロー";
                    }
                case FollowType.BlockAndBlockRelease:
                    {
                        return "B&BR済み";
                    }
                case FollowType.NotSet:
                    {
                        return "未設定";
                    }
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
