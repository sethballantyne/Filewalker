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
    public partial class FileEnumerator : Form
    {
        int fileCount;
        int directoryCount;

        string selectedPath;

        ImageList imageList;

        List<ListViewItem> listViewItems;

        public FileEnumerator()
        {
            InitializeComponent();
        }

        public FileEnumerator(string selectedPath, ImageList imageList) : this()
        {
            this.imageList = imageList;
            this.selectedPath = selectedPath;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Fires a RunWorkerCompleted event which is handled below in 
            // backgroundWorker_RunWorkerCompleted
            backgroundWorker.CancelAsync();

            Close();
        }

        private void FileEnumerator_Shown(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker.RunWorkerAsync();
            }
            catch
            {
                MessageBox.Show("Exception caught");
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
                DirectoryInfo dirInfo = new DirectoryInfo(selectedPath);

                FileInfo[] files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

                // + 1 because when the user sets a directory that doesn't contain 
                // any subdirectories, dirInfo.GetDirectories().Length will return 0.
                // directoryCount = dirInfo.GetDirectories().Length + 1;
                directoryCount = dirInfo.GetDirectories("*", SearchOption.AllDirectories).Length + 1;
                fileCount = files.Length;

                //string[] items = new string[listView.Columns.Count];
                string[] items = new string[4];
                listViewItems = new List<ListViewItem>(files.Length);

                //listView.Items.Clear();
                foreach (FileInfo file in files)
                {
                    string filePath = file.DirectoryName + @"\" + file.Name;

                    items[0] = file.Name;
                    items[1] = file.DirectoryName;
                    items[2] = FileSizeConverter.Format(file.Length);
                    items[3] = File.GetCreationTime(filePath).ToString();

                    string fileExtension = Path.GetExtension(filePath);
                    if (imageList.Images[fileExtension] == null)
                    {
                        Icon associatedIcon = ShellIcon.GetSmallIcon(file.DirectoryName + "\\" + file.Name);

                        // make sure we actually retrieved the files icon; passing a null
                        // value to the image list will result in an exception being thrown.
                        if (associatedIcon != null)
                            imageList.Images.Add(fileExtension, associatedIcon);
                    }

                    //listViewItems.Add(new ListViewItem()
                    listViewItems.Add(new ListViewItem(items, fileExtension));
                }

                backgroundWorker.ReportProgress(100);
        }

        // Raised when the DoWork handler returns, either because of error, 
        // or after DoWork() has successfully completed.
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Unholy error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;

                return;
            }
            else if (e.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListViewItem[] GetProcessedItems()
        {
            if(listViewItems != null)
            {
                return listViewItems.ToArray();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public int DirectoryCount
        {
            get { return directoryCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int FileCount
        {
            get { return fileCount;  }
        }
    }
}
