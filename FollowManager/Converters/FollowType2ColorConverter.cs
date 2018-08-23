using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FollowManager.Account;

namespace FollowManager.Converters
{
    public class FollowType2ColorConverter : IValueConverter
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
                    // 水色
                    return new SolidColorBrush((Color)new ColorConverter().ConvertFrom("#03A9F4"));
                case FollowType.Fan:
                    // ピンク
                    return new SolidColorBrush((Color)new ColorConverter().ConvertFrom("#FF4081"));
                case FollowType.Mutual:
                    // オレンジ
                    return new SolidColorBrush((Color)new ColorConverter().ConvertFrom("#FF5722"));
                case FollowType.BlockAndBlockRelease:
                    // 紫
                    return new SolidColorBrush((Color)new ColorConverter().ConvertFrom("#E040FB"));
                case FollowType.NotSet:
                    // ブルーグレー
                    return new SolidColorBrush((Color)new ColorConverter().ConvertFrom("#607D8B"));
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
