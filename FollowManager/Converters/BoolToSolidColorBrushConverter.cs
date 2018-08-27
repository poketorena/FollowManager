using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FollowManager.Converters
{
    public class BoolToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(bool))
            {
                throw new ArgumentException();
            }

            if ((bool)value)
            {
                // ピンク
                return new SolidColorBrush((Color)new ColorConverter().ConvertFrom("#E91E63"));
            }
            else
            {
                // 灰色
                return new SolidColorBrush((Color)new ColorConverter().ConvertFrom("#616161"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
