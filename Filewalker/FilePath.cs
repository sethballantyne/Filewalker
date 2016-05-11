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
using System.IO;

namespace Filewalker
{
    /// <summary>
    /// Utility class used by the dialog that copies files. Just makes it a bit easier
    /// to deal with files and their paths. 
    /// </summary>
    public class FilePath
    {
        string filename;

        string path;

        /// <summary>
        /// Initiaises the object with the specified path and filename.
        /// </summary>
        /// <param name="filename">The name of the file the object represents.</param>
        /// <param name="path">The full path of the file, minus the filename.</param>
        /// <exception cref="System.ArgumentException"><i>filename</i> or <i>path</i> contain illegal characters, 
        /// or either argument is a zero length string.</exception>
        /// <exception cref="System.ArgumentNullException"><i>filename</i> or <i>path</i> are <b>null</b>.</exception>
        public FilePath(string filename, string path)
        {
            if (filename == null || path == null)
            {
                throw new ArgumentNullException();
            }

            if ((filename.Length == 0 ||
                path.Length == 0 ||
                filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars(), 0) != -1) ||
                path.IndexOfAny(System.IO.Path.GetInvalidPathChars(), 0) != -1)
            {
                throw new ArgumentException();
            }

            this.filename = filename;
            this.path = path;
        }

        /// <summary>
        /// Returns the filename assigned when the object was initialised.
        /// </summary>
        public string Filename
        {
            get { return filename; }
        }

        /// <summary>
        /// Returns the directory assigned when the object was initialised.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Returns a string consisting of both the assigned directory and filename.
        /// </summary>
        public string AbsolutePath
        {
            get { return System.IO.Path.Combine(path, filename); }
        }
    }
}
