using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace CPM
{
    /// <summary>
    /// RAW disk manager
    /// </summary>
    public class Drive
    {
        #region Constants/Structs

        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILEsHARE_READ = 0x00000001;
        public const uint FILEsHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 3;
        public const uint IOCTLdISK_GETdRIVE_GEOMETRY = 0x70000;
        public const uint FSCTL_DISMOUNT_VOLUME = 0x00090020;

        // Struct to store the disk information
        public struct DISK_GEOMETRY
        {
            public long Cylinders;
            public short MediaType;
            public int TracksPerCylinder;
            public int SectorsPerTrack;
            public int BytesPerSector;
        }

        #endregion 

        #region Members

        public string physicalName;
        public string RelativePath;
        public DISK_GEOMETRY diskGeometry;
        private FileStream stream;
        private SafeFileHandle driveHndl;
        
        #endregion

        #region DLL Imports & Kernel32 Related items
        
        // Create file is needed for getting the drive info
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(string lpFileName, System.UInt32 dwDesiredAccess,
                                                        System.UInt32 dwShareMode, IntPtr pSecurityAttributes, System.UInt32 dwCreationDisposition,
                                                        System.UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        // Needed for sending command to device for information retrieval
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize,
                                           IntPtr lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="physicalName"></param>
        /// <param name="RelativePath"></param>
        /// <exception cref="Exception"></exception>
        public Drive(string physicalName, string RelativePath)
        {
            if (string.IsNullOrEmpty(physicalName))
            {
                throw new Exception("Invalid drive name");
            }

            this.physicalName = physicalName;
            this.RelativePath = RelativePath;

            // Create a file to connect with the physical drive
            driveHndl = CreateFile(@"\\.\" + physicalName, GENERIC_READ | GENERIC_WRITE, (FILEsHARE_READ | FILEsHARE_WRITE), IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            // Get the drive geometry
            diskGeometry = new DISK_GEOMETRY();

            // Check if the handle is valid that we just created
            if (driveHndl.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            } else
            {
                // Open and save a file stream for reading/writing later
                stream = new FileStream(driveHndl, FileAccess.ReadWrite);

                // Allocate memory for the struct
                int geoStructSize = Marshal.SizeOf(typeof(DISK_GEOMETRY));
                IntPtr geoStruct = Marshal.AllocHGlobal(geoStructSize);
                int lpBytesReturned = 0;

                // Run the command to populate the buffer with the geometry data
                if (DeviceIoControl(driveHndl, IOCTLdISK_GETdRIVE_GEOMETRY, geoStruct, geoStructSize, geoStruct, geoStructSize, out lpBytesReturned, IntPtr.Zero))
                {
                    diskGeometry = (DISK_GEOMETRY)Marshal.PtrToStructure(geoStruct, typeof(DISK_GEOMETRY));
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the number of bytes that is specified and returns the data 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="numBytes"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int offset, int numBytes)
        {
            byte[] buffer = new byte[numBytes];

            // Go to the sector that we want to read
            stream.Seek(offset, SeekOrigin.Begin);
            stream.Read(buffer, 0, numBytes);

            return buffer;
        }

        /// <summary>
        /// Write the number of bytes that is specified from the provided data buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="numBytes"></param>
        /// <returns></returns>
        public void WriteBytes(byte[] buffer, int offset, int numBytes)
        {
            // Get partition info for this physical drive
			string partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", RelativePath);
    		ManagementObjectSearcher partitionQuery = new ManagementObjectSearcher(partitionQueryText);
			foreach (ManagementObject p in partitionQuery.Get())
			{
                // Get logical drive info for this partition
                string logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                ManagementObjectSearcher logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
				foreach (ManagementObject ld in logicalDriveQuery.Get())
				{
					string driveName = Convert.ToString(ld.Properties["Name"].Value); 

                    // Create a file to connect with the partition/volume
                    SafeFileHandle volumeHndl = CreateFile(@"\\.\" + driveName, GENERIC_READ | GENERIC_WRITE, (FILEsHARE_READ | FILEsHARE_WRITE), IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

                    // Dismount volume on this drive
                    int unused;
                    DeviceIoControl(volumeHndl, FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out unused, IntPtr.Zero);
                }
            }

            // Go to the location that we want to write
            stream.Seek(offset, SeekOrigin.Begin);

            // Write bytes
            stream.Write(buffer, 0, numBytes);
        }

        /// <summary>
        /// Close drive/stream
        /// </summary>
        public void Close()
        {
            if (stream != null)
            {
                stream.Close();
            }

            if (driveHndl != null && !driveHndl.IsClosed)
            {
                driveHndl.Close();
            }
        }

        #endregion
    }
}
