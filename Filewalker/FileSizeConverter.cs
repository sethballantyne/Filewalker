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

namespace Filewalker
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileSizeConverter
    {
        /// <summary>
        /// Takes a value in bytes and converts it to a formatted string.
        /// </summary>
        /// <param name="bytes">the amount in bytes to format.</param>
        /// <returns>A formatted string using the appropriate measurement. 
        /// Example: if <i>bytes</i> contains 300 the string returned will be <i>300 B</i>.
        /// if it contained 40324, the returned string would be <i>40.32 KB</i>.
        /// </returns>
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
                // 1MB = 1,048,576 bytes
                result = (double) bytes / 1048576;
                str = String.Format("{0:F}", result);
                return str + " MB";
            }
        }
    }
}
