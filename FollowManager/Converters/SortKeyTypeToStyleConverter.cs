using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FollowManager.FilterAndSort;

namespace FollowManager.Converters
{
    public class SortKeyTypeToStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
