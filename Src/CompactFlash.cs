using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CPM
{
    /// <summary>
    /// RAW disk manager
    /// </summary>
    public class CompactFlash
    {
        /// <summary>
        /// Unsafe stream manager
        /// </summary>
        public struct SafeStreamManager
        {
            /// <summary>
            /// Stream
            /// </summary>
            public Stream STR;
            /// <summary>
            /// Save file handle
            /// </summary>
            public Microsoft.Win32.SafeHandles.SafeFileHandle SH;
            /// <summary>
            /// Error flag
            /// </summary>
            public bool f_error;
        }

        const int MAX_ATTEMPTS_NUMBER = 4;

        // Import function
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Microsoft.Win32.SafeHandles.SafeFileHandle CreateFile(
            string lpFileName,
            FileAccess dwDesiredAccess,
            FileShare dwShareMode,
            uint lpSecurityAttributes,
            FileMode dwCreationDisposition,
            uint dwFlagsAndAttributes,
            uint hTemplateFile
        );

        /// <summary>
        /// Create safe file stream
        /// </summary>
        /// <param name="drive">Driver name</param>
        /// <param name="type">Fila access type</param>
        /// <returns></returns>
        public static unsafe SafeStreamManager CreateStream(string drive, FileAccess type)
        {
            SafeStreamManager retVal = new SafeStreamManager();
            Microsoft.Win32.SafeHandles.SafeFileHandle h;

            int n_attempts = 1;
            while (n_attempts < MAX_ATTEMPTS_NUMBER)
            {
                h = CreateFile(drive, type, FileShare.None, 0, FileMode.Open, 0, 0);
                if (!h.IsInvalid)
                {
                    Stream inPutFile = new FileStream(h, type);
                    retVal.SH = h;
                    retVal.STR = inPutFile;
                    retVal.f_error = false;
                    return retVal;
                }
                else
                    n_attempts++;
            }

            // Number of attempts over the MAX, CreateFileError
            retVal.f_error = true;
            return retVal;
        }

        /// <summary>
        /// Read bytes from DISK
        /// </summary>
        /// <param name="startingbyte">Byte offset from begin</param>
        /// <param name="numberOfBytes"></param>
        /// <param name="safeStreamManager"></param>
        /// <returns>Read data array</returns>
        /// <remarks>
        /// You can read a maximum of 2GB block (2.147.483.648 bytes)
        /// </remarks>
        public static unsafe byte[] ReadBytes(long startingbyte, int numberOfBytes, SafeStreamManager safeStreamManager)
        {
            byte[] data = new byte[numberOfBytes];
            if (!safeStreamManager.SH.IsInvalid)
            {
                if (safeStreamManager.STR.CanRead)
                {
                    safeStreamManager.STR.Seek(startingbyte, SeekOrigin.Begin);
                    safeStreamManager.STR.Read(data, 0, numberOfBytes);
                    return data;
                }
            }
            return null;
        }

        /// <summary>
        /// Read block from DISK
        /// </summary>
        /// <param name="startingBlock"></param>
        /// <param name="numberOfBlocks"></param>
        /// <param name="blockSize"></param>
        /// <param name="safeStreamManager"></param>
        /// <returns>Read data array</returns>
        /// <remarks>
        /// You can read a maximum of 2GB block (4.194.304 blocks)
        /// </remarks>
        public static unsafe byte[] ReadBlocks(long startingBlock, int numberOfBlocks, int blockSize, SafeStreamManager safeStreamManager)
        {
            byte[] data = new byte[numberOfBlocks * blockSize];
            if (!safeStreamManager.SH.IsInvalid)
            {
                if (safeStreamManager.STR.CanRead)
                {
                    safeStreamManager.STR.Seek(startingBlock * blockSize, SeekOrigin.Begin);
                    safeStreamManager.STR.Read(data, 0, numberOfBlocks * blockSize);
                    return data;
                }
            }
            return null;
        }

        /// <summary>
        /// Write bytes to DISK
        /// </summary>
        /// <param name="startingByte"></param>
        /// <param name="numberOfbytes"></param>
        /// <param name="data"></param>
        /// <param name="safeStreamManager"></param>
        /// <returns>0 ok -1 error</returns>
        /// <remarks>
        /// You can write a maximum of 2GB block (4.194.304 blocks)
        /// </remarks>
        public static unsafe int WriteBytes(long startingByte, int numberOfbytes, byte[] data, SafeStreamManager safeStreamManager)
        {
            if (!safeStreamManager.SH.IsInvalid)
            {
                if (safeStreamManager.STR.CanRead)
                {
                    safeStreamManager.STR.Seek(startingByte, SeekOrigin.Begin);
                    safeStreamManager.STR.Write(data, 0, numberOfbytes);
                    safeStreamManager.STR.Flush();
                    return 0;
                }
            }
            return -1;
        }


        /// <summary>
        /// Write blocks to DISK
        /// </summary>
        /// <param name="startingBlock"></param>
        /// <param name="numberOfBlocks"></param>
        /// <param name="blockSize"></param>
        /// <param name="data"></param>
        /// <param name="safeStreamManager"></param>
        /// <returns>0 ok -1 error</returns>
        /// <remarks>
        /// You can write a maximum of 2GB block (4.194.304 blocks)
        /// </remarks>
        public static unsafe int WriteBlocks(long startingBlock, int numberOfBlocks, int blockSize, byte[] data, SafeStreamManager safeStreamManager)
        {
            if (!safeStreamManager.SH.IsInvalid)
            {
                if (safeStreamManager.STR.CanRead)
                {
                    safeStreamManager.STR.Seek(startingBlock * blockSize, SeekOrigin.Begin);
                    safeStreamManager.STR.Write(data, 0, numberOfBlocks * blockSize);
                    safeStreamManager.STR.Flush();
                    return 0;
                }
            }
            return -1;
        }

        /// <summary>
        /// Drop the stream
        /// </summary>
        /// <param name="safeStreamManager"></param>
        /// <returns>true=OK false=error</returns>
        public static unsafe bool DropStream(SafeStreamManager safeStreamManager)
        {
            try
            {
                safeStreamManager.STR.Close();
                safeStreamManager.SH.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
