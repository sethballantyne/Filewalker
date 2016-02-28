using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewalker
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileSizeConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Format(long bytes)
        {
            double result;
            string str;

            // is the file size less than 1 KB?
            if (bytes < 1024)
            {
                return bytes + " B";
            }

            // is the file size less than 1MB?
            else if (bytes < 1048576) // 1MB
            {
                result = (double) bytes / 1024;
                str = String.Format("{0:F}", result);
                return str + " KB";
            }
            
            // Assume the file is Megabytes in size.
            else
            {
                result = (double) bytes / 1048576;
                str = String.Format("{0:F}", result);
                return str + " MB";
            }
        }
    }
}
