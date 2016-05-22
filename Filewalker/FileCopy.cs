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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Filewalker
{
    /// <summary>
    /// Handles to copying of one or more files to a selected directory.
    /// </summary>
    public partial class FileCopy : Form
    {
        /// <summary>
        /// The files to copy to <i>destinationDir</i>
        /// </summary>
        FilePath[] filesToCopy = null;

        /// <summary>
        /// The directory each file will be copied to. 
        /// </summary>
        string destinationDir = null;

        /// <summary>
        /// 
        /// </summary>
        long totalBytes;

        /// <summary>
        /// total number of files copied. This is used as feedback to the user.
        /// </summary>
        int numFilesProcessed = 0;

        public FileCopy()
        {
            InitializeComponent();

            // setting here instead of the designer 
            // because the values in the designer are meant
            // to be helpers.
            taskLabel.Text = "";
            fileLabel.Text = "";
        }

        /// <summary>
        /// Initialises a new instance of FileCopy. 
        /// </summary>
        /// <param name="files">The absolute paths of the all the files to be copied.</param>
        /// <param name="destination">the target directory the file will be copied to.</param>
        /// <param name="totalBytes">the total size in bytes of all the files in <i>files</i></param>
        /// <exception cref="System.ArgumentNullException"><i>files</i> is <b>null</b> or <i>destination</i>
        /// is <b>null</b> or is a zero-length string.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><i>totalBytes</i> is less than or equal to 0.</exception>
        public FileCopy(FilePath[] files, string destination, long totalBytes) : this()
        {
            if (files == null)
                throw new ArgumentNullException("files");

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentNullException("destination");

            if (totalBytes <= 0)
                throw new ArgumentOutOfRangeException("totalBytes");

            filesToCopy = files;
            destinationDir = destination;
            this.totalBytes = totalBytes;
            
            // TODO: This shouldn't be here. Put it in a Shown handler instead. 
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 1MB buffer
            byte[] buffer = new byte[1024 * 1024];

            // used to update the progressbar as data is read in. Basically a counter for how
            // much data has been read in at a given time. 
            long bytesProcessed = 0;

            // used to store the number of bytes read in a given iteration of the loop.
            // won't exceed the size of buffer. The number is added to bytesProcessed,
            // before more data is read in. 
            int amountOfDataRead;

            foreach(FilePath file in filesToCopy)
            {
                using(FileStream sourceFile = new FileStream(file.AbsolutePath, FileMode.Open, FileAccess.Read))
                {
                    bytesProcessed = 0;
                    
                    string destinationPath = Path.Combine(destinationDir, file.Filename);

                    using(FileStream destinationFile = new FileStream(destinationPath, FileMode.OpenOrCreate, 
                        FileAccess.Write))
                    {
                        // update the dialog so it displays the correct filename 
                        // *before* we start reading data. 
                        backgroundWorker.ReportProgress(0, file.Filename);


                        while((amountOfDataRead = sourceFile.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if(backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            bytesProcessed += amountOfDataRead;
                            
                            int percentage = Convert.ToInt32(bytesProcessed * 100.0 / sourceFile.Length);

                            destinationFile.Write(buffer, 0, amountOfDataRead);

                            // looks like we're updating only one bar, but it's actually both.
                            backgroundWorker.ReportProgress(percentage, file.Filename);
                        }
                    }
                }

                numFilesProcessed++;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string noun;

            taskProgressBar.Value = Convert.ToInt32(numFilesProcessed * 100.0 / filesToCopy.Length);
            fileProgressBar.Value = e.ProgressPercentage;

            if (numFilesProcessed != 1)
                noun = "files";
            else
                noun = "file";

            fileLabel.Text = "Copying file: " + e.UserState as string;
            taskLabel.Text = String.Format("{0} {1} of {2} copied", numFilesProcessed, noun, filesToCopy.Length);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Unholy error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;

                return;
            }
            else if(e.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FileCopy_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }
    }
}
