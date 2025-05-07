using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Quiz.ViewModel
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
           value is bool boolValue && boolValue ? Visibility.Visible : Visibility.Collapsed; // nie dzialało mi wcześniej i musiałam coś zmienić
                                                                                            // w tym pliku, ktory ty zrobiłaś, ale mam nadzieję, 
                                                                                            // że nic ci to nie zepsuło, najlepiej sprawdzić

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
