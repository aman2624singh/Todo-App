using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Converters
{
    public class FilePathToImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine($"Converter called with value: {value}");

            if (value is string filePath && !string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    return ImageSource.FromFile(filePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error converting file path to image source: {ex.Message}");
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
