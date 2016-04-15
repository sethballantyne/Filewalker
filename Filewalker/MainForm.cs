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
    public partial class MainForm : Form
    {
        //
        ListViewColumnSorter listViewColumnSorter = new ListViewColumnSorter();

        //
        string selectedDirectory = null;

        //
        int directoryCount = 0;

        //
        int fileCount = 0;

        public MainForm()
        {
            InitializeComponent();

            // doing it here instead of the form designer so the
            // control is more easily visible in the designer.
            toolStripStatusLabel.Text = "";

            listView.ListViewItemSorter = listViewColumnSorter;
            listView.SmallImageList = imageList;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChooseDirectory()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                selectedDirectory = folderBrowserDialog.SelectedPath;

                EnumerateFiles();

                refreshToolStripMenuItem.Enabled = true;
                refreshToolStripButton.Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void EnumerateFiles()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);

            FileInfo[] files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

            // + 1 because when the user sets a directory that doesn't contain 
            // any subdirectories, dirInfo.GetDirectories().Length will return 0.
            this.directoryCount = dirInfo.GetDirectories().Length + 1;
            this.fileCount = files.Length;

            string[] items = new string[listView.Columns.Count];

            List<ListViewItem> listViewItems = new List<ListViewItem>(files.Length);

            listView.Items.Clear();
            foreach (FileInfo file in files)
            {
                string filePath = file.DirectoryName + @"\" + file.Name;

                items[0] = file.Name;
                items[1] = file.DirectoryName;
                items[2] = FileSizeConverter.Format(file.Length);
                items[3] = File.GetCreationTime(filePath).ToString();
                

                string fileExtension = Path.GetExtension(filePath);
                if(imageList.Images[fileExtension] == null)
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

            listView.Items.AddRange(listViewItems.ToArray());

            UpdateStatusLabel();
        }

        /// <summary>
        /// Sets the Enabled property of each item in the listview context menu.
        /// </summary>
        /// <param name="enabled">The value to assign to the items Enabled property.</param>
        private void ToggleContextMenuItems(bool enabled)
        {
            for (int i = 0; i < contextMenuStrip.Items.Count; i++)
            {
                contextMenuStrip.Items[i].Enabled = enabled;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetSelectedFilePath()
        {
            string filePath = listView.SelectedItems[0].SubItems[1].Text +
                "\\" +
                listView.SelectedItems[0].Text;

            return filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateStatusLabel()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0} - {1} files in ", this.selectedDirectory, this.fileCount);
            if (directoryCount == 1)
            {
                stringBuilder.AppendFormat("1 directory.");
            }
            else
            {
                stringBuilder.AppendFormat("{0} directories.", this.directoryCount);
            }

            toolStripStatusLabel.Text = stringBuilder.ToString();
        }

        private void chooseDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChooseDirectory();
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == listViewColumnSorter.SortColumn)
            {
                if (listViewColumnSorter.SortingOrder == SortOrder.Ascending)
                {
                    listViewColumnSorter.SortingOrder = SortOrder.Descending;
                }
                else
                {
                    listViewColumnSorter.SortingOrder = SortOrder.Ascending;
                }
            }

            else
            {
                // Set the column number that is to be sorted; default to ascending.
                listViewColumnSorter.SortColumn = e.Column;
                listViewColumnSorter.SortingOrder = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            listView.Sort();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                ToggleContextMenuItems(true);
            }
            else
            {
                ToggleContextMenuItems(false);
            }
        }

        private void copyToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView.SelectedItems.Count == 0)
            {
                return;
            }

            // directory path + filename
            string pathOfFileToCopy = listView.SelectedItems[0].SubItems[1].Text + 
                "\\" +
                listView.SelectedItems[0].Text;

            // TODO: Move this to its own thread, indicate file copy progress.
            saveFileDialog.FileName = listView.SelectedItems[0].Text;
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(pathOfFileToCopy, saveFileDialog.FileName, true);

                MessageBox.Show("File Copied.");
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView.SelectedItems.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Delete selected file?",
                    "Confirm action",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if(dialogResult == DialogResult.Yes)
                {
                    string pathOfFileToDelete = GetSelectedFilePath();

                    File.Delete(pathOfFileToDelete);

                    listView.Items.Remove(listView.SelectedItems[0]);

                    fileCount--;

                    MessageBox.Show("File deleted.");

                    UpdateStatusLabel();
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnumerateFiles();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(GetSelectedFilePath());
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().Show();
        }

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            EnumerateFiles();
        }

        private void setPathToolStripButton_Click(object sender, EventArgs e)
        {
            ChooseDirectory();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(GetSelectedFilePath());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "Are you sure you wish to exit?",
                "Continue?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if(dr == DialogResult.Yes)
            {
                Close();
            }
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = listView.SelectedItems[0].SubItems[1].Text;
            psi.Verb = "open";

            System.Diagnostics.Process.Start(psi);
        }
    }
}
