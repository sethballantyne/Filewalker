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
using System.Diagnostics;

namespace Filewalker
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DeleteFilesDlg : Form
    {
        //
        ListViewItem[] filesToDelete = null;

        //
        int currentIndex = 0;

        public DeleteFilesDlg()
        {
            InitializeComponent();

            // setting here instead of the designer 
            // because the values in the designer are meant
            // to be helpers.
            label.Text = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filesToDelete"></param>
        public DeleteFilesDlg(ListViewItem[] filesToDelete)
            : this()
        {
            this.filesToDelete = filesToDelete;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for(int i = 0; i < filesToDelete.Length; i++)
            {
                currentIndex = i;

                // Allow the user to cancel. If you don't do this, clicking cancel won't
                // do anything!
                if(backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                int percentage = Convert.ToInt32((i + 1) * 100.0 / filesToDelete.Length);
                
                string fileToDelete = Path.Combine(filesToDelete[i].SubItems[1].Text,
                    filesToDelete[i].Text);

                File.Delete(fileToDelete);
                
                backgroundWorker.ReportProgress(percentage, fileToDelete);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
                label.Text = "deleting " + e.UserState as string;
                progressBar.Value = e.ProgressPercentage;            
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                // Handle error
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

        private void DeleteFilesDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if backgroundWorker_DoWork is running, cancel it.
            if(backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void DeleteFilesDlg_Shown(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
        }
    }
}
