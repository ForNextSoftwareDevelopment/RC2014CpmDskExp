using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management;

namespace CPM
{
    /// <summary>
    /// Disk indexer
    /// </summary>
    public class Drives
    {
		#region Members

		public class Disk
		{
			public string physicalName;
            public string RelativePath;
            public string mediaType;
            public string diskName;
            public string driveName;

			public Disk(string physicalName, string RelativePath, string mediaType, string diskName, string driveName)
			{
				this.physicalName = physicalName;
                this.RelativePath = RelativePath;
                this.mediaType = mediaType;
                this.diskName = diskName;
                this.driveName = driveName;
			}
        }

		public List<Disk> disks; 

		#endregion

		#region Constructor/Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Drives()
		{
			disks = new List<Disk>();

			// Get physical drive info
            ManagementObjectSearcher driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
			foreach (ManagementObject d in driveQuery.Get())
			{
                string deviceId = Convert.ToString(d.Properties["DeviceId"].Value); // \\.\PHYSICALDRIVE2
                string physicalName = Convert.ToString(d.Properties["Name"].Value); // \\.\PHYSICALDRIVE2
                string diskName = Convert.ToString(d.Properties["Caption"].Value); // WDC WD5001AALS-xxxxxx
                string diskModel = Convert.ToString(d.Properties["Model"].Value); // WDC WD5001AALS-xxxxxx
                string diskInterface = Convert.ToString(d.Properties["InterfaceType"].Value); // IDE
                UInt16[] capabilities = (UInt16[])d.Properties["Capabilities"].Value; // 3,4 - random access, supports writing
                bool mediaLoaded = Convert.ToBoolean(d.Properties["MediaLoaded"].Value); // bool
                string mediaType = Convert.ToString(d.Properties["MediaType"].Value); // Fixed hard disk media
                UInt32 mediaSignature = Convert.ToUInt32(d.Properties["Signature"].Value); // UInt32
                string mediaStatus = Convert.ToString(d.Properties["Status"].Value); // OK
                UInt64 totalSectors = Convert.ToUInt64(d.Properties["TotalSectors"].Value); // UInt64
                UInt32 BytesPerSector = Convert.ToUInt32(d.Properties["BytesPerSector"].Value); // UInt32

                // Add to list
                Disk disk = new Disk(physicalName, d.Path.RelativePath, mediaType, diskName, "");
                disks.Add(disk);

                // Get partition info for this physical drive
				string partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", d.Path.RelativePath);
    			ManagementObjectSearcher partitionQuery = new ManagementObjectSearcher(partitionQueryText);
				foreach (ManagementObject p in partitionQuery.Get())
				{
                    // Get logical drive info for this partition
                    string logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                    ManagementObjectSearcher logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
					foreach (ManagementObject ld in logicalDriveQuery.Get())
					{
						string driveName = Convert.ToString(ld.Properties["Name"].Value); // C:
						string driveId = Convert.ToString(ld.Properties["DeviceId"].Value); // C:
						bool driveCompressed = Convert.ToBoolean(ld.Properties["Compressed"].Value);
						UInt32 driveType = Convert.ToUInt32(ld.Properties["DriveType"].Value); // C: - 3
						string fileSystem = Convert.ToString(ld.Properties["FileSystem"].Value); // NTFS
						UInt64 freeSpace = Convert.ToUInt64(ld.Properties["FreeSpace"].Value); // in bytes
						UInt64 totalSpace = Convert.ToUInt64(ld.Properties["Size"].Value); // in bytes
						UInt32 driveMediaType = Convert.ToUInt32(ld.Properties["MediaType"].Value); // c: 12
						string volumeName = Convert.ToString(ld.Properties["VolumeName"].Value); // System
						string volumeSerial = Convert.ToString(ld.Properties["VolumeSerialNumber"].Value); // 12345678

                        // Add driveName
                        for (int i = 0; i < disks.Count; i++)
						{
                            if (disks[i].physicalName == disk.physicalName)
							{
                                disks[i].driveName += " (";
                                disks[i].driveName += driveName;
                                disks[i].driveName += " )";
                            }
                        }
					}
				}
			}
        
            // Check driveName, if none found then indicate so (no partitions)
            for (int i = 0; i < disks.Count; i++)
            {
                if (disks[i].driveName == "")
                {
                    disks[i].driveName = disks[i].diskName + " (NO PARTITIONS)";
                } else
                {
                    disks[i].driveName = disks[i].diskName + disks[i].driveName;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all drives from a certain type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Dictionary<Drive, string> GetDrives(string type)
        {
            Dictionary<Drive, string> type_drives = new Dictionary<Drive, string>();

            for (int i = 0; i < disks.Count; i++)
            {
                // Check type and add to list
                if (disks[i].mediaType == type)
                {
                    Drive drive = new Drive(disks[i].physicalName, disks[i].RelativePath);
                    type_drives.Add(drive, disks[i].driveName);
                }
            }

            return type_drives;
        }
        
        #endregion
    }
}