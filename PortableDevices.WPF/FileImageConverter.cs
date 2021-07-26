using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PortableDevices.WPF
{
    public class FileImageConverter : IValueConverter
    {
        public static FileImageConverter Instance = new FileImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PortableDeviceFile.FileType type)
            {
                switch (type)
                {
                    case PortableDeviceFile.FileType.Document:
                        return new BitmapImage(new Uri("/resources/file.png", UriKind.Relative));
                        
                    case PortableDeviceFile.FileType.Image:
                        return new BitmapImage(new Uri("/resources/image.png", UriKind.Relative));
                        
                    case PortableDeviceFile.FileType.Movie:
                        return new BitmapImage(new Uri("/resources/movie.png", UriKind.Relative));
                }
            }
            return new BitmapImage(new Uri("/resources/file.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
