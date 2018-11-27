using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NicoV4.Mvvm.Converters
{
    public class Long2TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long l;
            if (value is double)
            {
                l = (long)((double)value);
            }
            else if (value is long)
            {
                l = (long)value;
            }
            else if (!long.TryParse(value.ToString(), out l))
            {
                throw new ArgumentException("not long", "value");
            }

            return new TimeSpan(l * 100 * 100 * 1000);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
