using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AmiiboAPIProject.View.Converters
{
    class TypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return new BitmapImage(new Uri("https://static.thenounproject.com/png/149589-200.png"));
            }

            if(value == null)
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/TypeIcons/figure.png"));
            }

            String type = value.ToString();
            return new BitmapImage(new Uri($"pack://application:,,,/Resources/Images/TypeIcons/{type}.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No need to implement it
            return null;
        }
    }
}
