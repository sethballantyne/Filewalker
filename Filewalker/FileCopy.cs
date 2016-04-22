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
    /// 
    /// </summary>
    public partial class FileCopy : Form
    {
        FilePath[] filesToCopy = null;
        string destinationDir = null;
        long totalBytes;

        int numFilesProcessed = 0;

        public FileCopy()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        public FileCopy(FilePath[] files, string destination, long totalBytes) : this()
        {
            filesToCopy = files;
            destinationDir = destination;
            this.totalBytes = totalBytes;
            
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 1MB buffer
            byte[] buffer = new byte[1024 * 1024];

            long bytesProcessed = 0;
            int currentBlockSize;

            foreach(FilePath file in filesToCopy)
            {
                using(FileStream sourceFile = new FileStream(file.AbsolutePath, FileMode.Open, FileAccess.Read))
                {
                    bytesProcessed = 0;
                    string destinationPath = destinationDir + "\\" + file.Filename;
                    
                    using(FileStream destinationFile = new FileStream(destinationPath, FileMode.OpenOrCreate, 
                        FileAccess.Write))
                    {
                        backgroundWorker.ReportProgress(0, file.Filename);

                        while((currentBlockSize = sourceFile.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if(backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            bytesProcessed += currentBlockSize;
                            
                            int percentage = Convert.ToInt32(bytesProcessed * 100.0 / sourceFile.Length);

                            destinationFile.Write(buffer, 0, currentBlockSize);

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

            if (numFilesProcessed == 1)
                noun = "file";
            else
                noun = "files";

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
