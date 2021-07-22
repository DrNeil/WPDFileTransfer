using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PortableDevices.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservablePortableDeviceCollection Devices
        {
            get { return (ObservablePortableDeviceCollection)GetValue(DevicesProperty); }
            set { SetValue(DevicesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Devices.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DevicesProperty =
            DependencyProperty.Register("Devices", typeof(ObservablePortableDeviceCollection), typeof(MainWindow), new PropertyMetadata(null));


        public MainWindow()
        {
            Devices = new ObservablePortableDeviceCollection();
            DataContext = this;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDevices();
        }

        private void UpdateDevices()
        {
            Devices.Refresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDevices();
        }
    }
}
