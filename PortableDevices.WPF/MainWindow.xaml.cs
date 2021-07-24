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



        public PortableDevice SelectedDevice
        {
            get { return (PortableDevice)GetValue(SelectedDeviceProperty); }
            set { SetValue(SelectedDeviceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDevice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDeviceProperty =
            DependencyProperty.Register("SelectedDevice", typeof(PortableDevice), typeof(MainWindow), new PropertyMetadata(null));



        public PortableDeviceFolder CurrentFolder
        {
            get { return (PortableDeviceFolder)GetValue(CurrentFolderProperty); }
            set { SetValue(CurrentFolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentFolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentFolderProperty =
            DependencyProperty.Register("CurrentFolder", typeof(PortableDeviceFolder), typeof(MainWindow), new PropertyMetadata(null));



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

        private void DevicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != SelectedDevice)
            {
                CurrentFolder = SelectedDevice.Root;
                SelectedDevice.Connect();
                SelectedDevice.GetFiles(CurrentFolder);
            }
        }

        protected void FileDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PortableDeviceObject selected = ((ListViewItem)sender).Content as PortableDeviceObject;
            if (selected is PortableDeviceFolder selectedFolder)
            {
                CurrentFolder = selectedFolder;
                SelectedDevice.Connect();
                SelectedDevice.GetFiles(CurrentFolder);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != CurrentFolder)
            {
                
            }
        }
    }
}
