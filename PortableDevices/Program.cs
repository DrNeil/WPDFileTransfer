using PortableDeviceApiLib;
using System;
using System.Diagnostics;
using System.Linq;

/**  
 * Create a dir on your c: drive named Text "C:\Test"
 * Put in any files that you desire.
 * 
 * After program runs:
 *   On Phone: Visit dir Phone\Android\data\test
 *   On PC:    Visit dir c:\Test\CopiedBackfromPhone
 *   
 * To test Folder Copying:
 *   Set COPY_FOLDER = true;
 *   
 * To test File Copying:
 *   Set COPY_FOLDER = false;
 *   And ensure that file: "C:\Test\foo.txt" exists
 */
namespace PortableDevices
{
    class Program
    {
        private static Boolean COPY_FOLDER = true;

        // https://cgeers.wordpress.com/2012/04/17/wpd-transfer-content-to-a-device/
        static void Main()
        {
            string error = string.Empty;
            PortableDeviceCollection devices = null;
            try
            {
                devices = new PortableDeviceCollection();
                if (null != devices)
                {
                    char cmdCharacter = ' ';
                    ListDevices(devices);
                    do
                    {
                        Console.WriteLine($"-------------------------------------");
                        Console.WriteLine("Select device by number for more details");
                        Console.WriteLine("Press r to refresh");
                        Console.WriteLine("Press x key to exit");

                        cmdCharacter = Console.ReadKey().KeyChar;
                        if (cmdCharacter == 'r')
                        {
                            ListDevices(devices);
                        }
                        else if (cmdCharacter > '0' && cmdCharacter <= '0' + devices.Count)
                        {

                            var device = devices[cmdCharacter - '1'];
                            if (null != device)
                            {
                                ShowDeviceDetails(device);
                                ListDevices(devices);
                            }

                        }
                    } while ('x' != cmdCharacter);

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (null != devices)
                {
                    devices.Dispose();
                }
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine(error);
                    Console.ReadKey();
                }
            }
        }

        private static void ShowDeviceDetails(PortableDevice device)
        {
            Console.WriteLine();
            Console.WriteLine($"{device.Name} from {device.Manufacturer}");
            Console.WriteLine($"\tDescription: {device.Description}");

            

            var folder = device.Root;

            
            // list all contents in the root - 1 level in 
            //see GetFiles method to enumerate everything in the device 
            int fileNo = 0;

            ListFilesInFolder(device, ref folder, ref fileNo);

            string cmd;
            do
            {
                Console.WriteLine($"-------------------------------------");
                Console.WriteLine("Select file by number ");
                Console.WriteLine("Press x key to exit to device list");
                cmd = Console.ReadLine();
                int selectedItem = 0;

                if (int.TryParse(cmd, out selectedItem)
                    && selectedItem <= folder.Files.Count)
                {
                    var fileItem = folder.Files[selectedItem - 1];
                    if (fileItem is PortableDeviceFolder childFolder)
                    {
                        fileNo = 0;
                        ListFilesInFolder(device, ref childFolder, ref fileNo);
                        folder = childFolder;
                    }
                    else if (fileItem is PortableDeviceFile childFile)
                    {
                        var store = System.IO.Path.GetTempPath();
                        device.TransferContentFromDevice(childFile, store, childFile.Name);
                        // open in default application
                        var path = System.IO.Path.Combine(store, childFile.Name);
                        Process.Start(path);
                        fileNo = 0;
                        ListFilesInFolder(device, ref folder, ref fileNo);
                    }
                }
            }
            while (cmd != "x");
            
        }

        private static void ListFilesInFolder(PortableDevice device, ref PortableDeviceFolder folder, ref int fileNo)
        {
            device.Connect();
            IPortableDeviceContent content = device.getContents();
            if (null == folder)
            {
                folder = device.Root;
            }
            PortableDeviceFolder.EnumerateContents(ref content, folder);
            Console.WriteLine();
            Console.WriteLine($"{folder.Name} contents:");
            foreach (var fileItem in folder.Files)
            {
                fileNo++;
                Console.WriteLine($"\t{fileNo}:\t{fileItem.Name}");
            }
            device.Disconnect();
        }

        private static void ListDevices(PortableDeviceCollection devices)
        {
            devices.Refresh();
            Console.Clear();
            Console.WriteLine($"Found {devices.Count} Device(s)");
            Console.WriteLine($"-------------------------------------");
            int deviceNo = 0;
            foreach (var device in devices)
            {
                deviceNo++;
                Console.WriteLine();
                Console.WriteLine($"{deviceNo}: {device.Name} from {device.Manufacturer}");


                // Copy folder to device from pc.
                //error = copyToDevice (device);
                //if (String.IsNullOrEmpty(error))
                //{
                //    error = @"Copied folder C:\Test to Phone\Android\data\test";
                //}
                //Console.WriteLine(error);

                //// Copy folder back to pc from device.
                //error = copyFromDevice(device);
                //if (String.IsNullOrEmpty(error))
                //{
                //    error = @"Copied folder Phone\Android\data\test to c:\Test\CopiedBackfromPhone";
                //}
            }
        }

        private static void GetFiles(ref IPortableDeviceContent content, PortableDeviceFolder folder)
        {
            PortableDeviceFolder.EnumerateContents(ref content, folder);
            foreach (var fileItem in folder.Files)
            {
                Console.WriteLine($"\t{fileItem.Name}");
                if (fileItem is PortableDeviceFolder childFolder)
                {
                    GetFiles(ref content, childFolder);
                }
            }
        }

        /**
         * Copy test file to device.
         */
        public static String copyToDevice (PortableDevice device)
        {
            String error = "";

            try
            {
                // Try to find the data folder on the phone.
                String               phoneDir = @"Phone\Android\data";
                PortableDeviceFolder root     = device.Root;
                PortableDeviceFolder result   = root.FindDir (phoneDir);
                if (null == result)
                {
                    // Perhaps it was a tablet instead of a phone?
                    result = device.Root.FindDir(@"Tablet\Android\data");
                    phoneDir = @"Tablet\Android\data";
                }

                // Did we find a the desired folder on the device?
                if (null == result)
                {
                    error = phoneDir + " not found!";
                }
                else
                {
                    // Create remote test folder.
                    result = result.CreateDir (device, "test");

                    string pcDir = @"C:\Test\";

                    if (COPY_FOLDER)
                    {
                        // copy a whole folder. public void CopyFolderToPhone (PortableDevice device, String folderPath, String destPhonePath)
                        result.CopyFolderToPhone(device, pcDir, phoneDir);
                    }
                    else
                    {
                        // Or Copy a single file. 
                        device.TransferContentToDevice (result, pcDir + "foo.txt");
                    }
                }

            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;
        }

        /**
         * Copy test file to device.
         */
        public static String copyFromDevice (PortableDevice device)
        {
            String error = "";

            try
            {
                PortableDeviceFolder root   = device.Root;
                PortableDeviceObject result = root.FindDir (@"Phone\Android\data\test");
                if (null == result)
                {
                    // Perhaps it was a tablet instead of a phone?
                    result = root.FindDir(@"Tablet\Android\data\test");
                }

                // Did we find a the desired folder on the device?
                if (null == result)
                {
                    error = @"Dir Android\data not found!";
                }
                else if (result is PortableDeviceFolder)
                {                    
                    if (COPY_FOLDER)
                    {
                        // Copy a whole folder
                        ((PortableDeviceFolder)result).CopyFolderToPC(device, @"C:\Test\CopiedBackfromPhone");
                    }
                    else
                    {
                        // Or Copy a file
                        PortableDeviceFile file = ((PortableDeviceFolder)result).FindFile("foo.txt");                    
                        device.TransferContentFromDevice (file, @"C:\Test\CopiedBackfromPhone", "Copyfoo.txt");
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;
        }
    }   
}
