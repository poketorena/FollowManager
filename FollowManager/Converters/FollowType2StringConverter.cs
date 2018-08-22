using System;
using System.Globalization;
using System.Windows.Data;
using FollowManager.Account;

namespace FollowManager.Converters
{
    public class FollowType2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(FollowType))
            {
                throw new ArgumentException();
            }

            switch ((FollowType)value)
            {
                case FollowType.Follow:
                    return "片思い";
                case FollowType.Follower:
                    return "ファン";
                case FollowType.Mutual:
                    return "相互フォロー";
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
