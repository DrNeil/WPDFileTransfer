using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        private Stack<PortableDeviceFolder> folderHistory;


        public MainWindow()
        {
            folderHistory = new Stack<PortableDeviceFolder>();
            Devices = new ObservablePortableDeviceCollection();
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
                folderHistory.Clear();
                CurrentFolder = SelectedDevice.Root;
                UpdateFilesInCurrentFolder();
            }
        }

        protected void FileDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PortableDeviceObject selected = ((ListViewItem)sender).Content as PortableDeviceObject;
            if (selected is PortableDeviceFolder selectedFolder)
            {
                CurrentFolder = selectedFolder;
                UpdateFilesInCurrentFolder();
            }
            else if (selected is PortableDeviceFile selectedFile)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = selectedFile.Name;
                if (saveFileDialog.ShowDialog() == true)
                {
                    var folder = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                    var file = System.IO.Path.GetFileName(saveFileDialog.FileName);
                    SelectedDevice.TransferContentFromDevice(selectedFile, folder, file);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != folderHistory
                && folderHistory.Count > 1)
            {
                CurrentFolder = folderHistory.Pop();
                CurrentFolder = folderHistory.Pop();
                UpdateFilesInCurrentFolder();
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            if (null != CurrentFolder)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    SelectedDevice.TransferContentToDevice(CurrentFolder, openFileDialog.FileName);
                    UpdateFilesInCurrentFolder();
                }
            }
        }
        
        private void UpdateFilesInCurrentFolder()
        {
            FileListView.ItemsSource = null; // hack as Files in PortableFolder is not observable - this forces a refresh of the list
            SelectedDevice.Connect();
            CurrentFolder = SelectedDevice.GetFiles(CurrentFolder);
            if (null != CurrentFolder)
            {
                if (folderHistory.Any()
                    && folderHistory.Peek() != CurrentFolder)
                {
                    folderHistory.Push(CurrentFolder);
                }
            }
            FileListView.ItemsSource = CurrentFolder.Files;
        }

    }
}
