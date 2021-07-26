// Portable Device File

namespace PortableDevices
{
    public class PortableDeviceFile : PortableDeviceObject
    {
        public enum FileType
        {
            Image,
            Movie,
            Document,
            GenericFile,
            Unknown
        }

        public long size = 0;
        public FileType Type { get; private set; }

        public PortableDeviceFile (string id, string name, long objSiz, FileType type = FileType.Unknown) : base(id, name)
        {
            size = objSiz;
            Type = type;
        }

        public string Path { get; set; }
    }
}