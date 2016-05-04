/*Copyright (c) 2016 Seth Ballantyne <seth.ballantyne@gmail.com>
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in
*all copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Filewalker
{
    /// <summary>
    /// Taken and modified from http://www.pinvoke.net/default.aspx/shell32.SHGetFileInfo
    /// Credit goes to whoever wrote the original code.
    /// </summary>
    public static class ShellIcon
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        /// <summary>
        /// Stores information about a file object; should be used in conjunction with
        /// the SHGetFileInfo function.s
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            /// <summary>
            /// Handle to the icon file. When the icon is no longer needed
            /// it must be destroyed with DestroyIcon.
            /// </summary>
            public IntPtr hIcon;

            /// <summary>
            /// The index of the icon image within the system image list.
            /// </summary>
            public IntPtr iIcon;

            /// <summary>
            /// A value that indicates the icons attributes. 
            /// We don't use this but it's here for completeness.
            /// </summary>
            public uint dwAttributes;

            /// <summary>
            /// The icons name as it appears in the Windows Shell,
            /// or the path and filename of the file that contains
            /// the icon representing the file.
            /// Defined as TCHAR[MAX_PATH] in the Win32 API.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            /// <summary>
            /// Describes the type of file. Defined as TCHAR[80] 
            /// in the Win32 API. 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        class Win32
        {
            // SHGetFileInfo supports flags other than the ones below
            // but they're considered unsupported for our purposes. 
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            [DllImport("shell32.dll")]
            public static extern UIntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);

        }

        /// <summary>
        /// Gets the small icon associated with the specified file.
        /// </summary>
        /// <param name="fileName">The path of the file whose icon is to be retrieved.</param>
        /// <returns>null on failure, else the icon stored in an instance of Icon</returns>
        public static Icon GetSmallIcon(string fileName)
        {
            return GetIcon(fileName, Win32.SHGFI_SMALLICON);
        }

        /// <summary>
        /// Gets the large icon associated with the specified file.
        /// </summary>
        /// <param name="fileName">The path of the file whose icon is to be retrieved.</param>
        /// <returns>null on failure, else the icon stored in an instance of Icon.</returns>
        public static Icon GetLargeIcon(string fileName)
        {
            return GetIcon(fileName, Win32.SHGFI_LARGEICON);
        }

        /// <summary>
        /// Retrieves the icon associated with the file at the specified path.
        /// </summary>
        /// <param name="fileName">The path of the file whose icon is to be retrieved.</param>
        /// <param name="flags">Specifies whether to retrieve the large icon (SHGFI_LARGEICON)
        /// or the small icon (SHGFI_SMALLICON) associated with the specified file.</param>
        /// <returns>null on failure, otherwise the desired icon stored in an Icon instance.</returns>
        /// <remarks>The method has been designed only to work with the SHGFI_LARGEICON
        /// and SHGFI_SMALLICON flags. Use any other flags is unsupported.</remarks>
        private static Icon GetIcon(string fileName, uint flags)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            // The original code taken from pinvoke was storing the return value
            // in an IntPtr; SHGetFileInfo returns DWORD_PTR, which is an unsigned long.
            UIntPtr result = Win32.SHGetFileInfo(
                fileName,                           // LPCTSTR pszPath
                0,                                  // DWORD dwFileAttributes
                ref shinfo,                         // SHFILEINFO *psfi
                (uint)Marshal.SizeOf(shinfo),       // UINT cbFileInfo
                Win32.SHGFI_ICON | flags            // UINT uFlags
                );

            // SHGetGileInfo will return a non-zero value on success, 
            // else returns 0 on failure. 
            if (result.ToUInt32() == 0)
            {
                return null;
            }

            Icon icon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);

            return icon;
        }
    }
}
