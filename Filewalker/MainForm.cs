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
using Microsoft.Win32;

namespace Filewalker
{
    public partial class MainForm : Form
    {
        //
        ListViewColumnSorter listViewColumnSorter = new ListViewColumnSorter();

        FixedSizedList<string> recentDirectories = new FixedSizedList<string>(10);

        //
        string selectedDirectory = null;

        //
        int directoryCount = 0;

        //
        int fileCount = 0;

        const string registryKey = "HKEY_CURRENT_USER" + "\\" + "Filewalker";


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
            try
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedDirectory = folderBrowserDialog.SelectedPath;

                    UpdateRecentDirectoryMenuItems();

                    EnumerateFiles();

                    refreshToolStripMenuItem.Enabled = true;
                    refreshToolStripButton.Enabled = true;
                    selectAllToolStripMenuItem.Enabled = true;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message + "\n\n" + e.StackTrace, 
                    "Unholy Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateRecentDirectoryMenuItems()
        {
            // if this path has been opened before, we don't want
            // to display it twice. 
            for (int i = 0; i < recentDirectories.Count; i++)
            {
                if (recentDirectories[i] == selectedDirectory)
                {
                    recentDirectories.RemoveAt(i);
                }
            }

            // need a trailing directory seperator for it to be
            // a valid URI. If you don't add this, the check will fail
            // in MainForm_Load and no menu items will appear.
            recentDirectories.Add(selectedDirectory + "//");

            CreateRecentDirectoryMenuItems();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EnumerateFiles()
        {
            try
            {
                FileEnumerator fileEnum = new FileEnumerator(selectedDirectory, imageList);
                DialogResult dr = fileEnum.ShowDialog();


                if (dr == DialogResult.OK)
                {
                    // update the number of directories and files processed
                    // to the statusstrip label an display the correct information
                    directoryCount = fileEnum.DirectoryCount;
                    fileCount = fileEnum.FileCount;

                    listView.Items.Clear();
                    ListViewItem[] items = fileEnum.GetProcessedItems();

                    if (items != null)
                        listView.Items.AddRange(items);

                    UpdateStatusLabel();
                }
            }
            catch
            {
                throw;
            }     
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

            editDeleteToolStripMenuItem.Enabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetSelectedFilePath()
        {
            return Path.Combine(listView.SelectedItems[0].SubItems[1].Text, listView.SelectedItems[0].Text);
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

            List<FilePath> filesToCopy = new List<FilePath>();

            
            // directory path + filename
            string pathOfFileToCopy = Path.Combine(listView.SelectedItems[0].SubItems[1].Text,
                listView.SelectedItems[0].Text);

            // TODO: Move this to its own thread, indicate file copy progress.
            //saveFileDialog.FileName = listView.SelectedItems[0].Text;
            if(copyFileFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                long totalBytes = 0;

                foreach (ListViewItem selectedItem in listView.SelectedItems)
                {
                    string filename = selectedItem.Text;
                    string directoryPath = selectedItem.SubItems[1].Text;

                    // verify a file in the selected directory doesn't contain
                    // a file of the same name.
                    if (File.Exists(Path.Combine(copyFileFolderBrowserDialog.SelectedPath, filename)))
                    {
                        string message = String.Format("The file {0} exists in the specified directory. Overwrite?", filename);
                       
                        DialogResult result = MessageBox.Show(message, "Overwrite exsiting file?", 
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if(result == DialogResult.Cancel)
                        {
                            return;
                        }
                        else if(result == DialogResult.No)
                        {
                            continue;
                        }
                    }

                    filesToCopy.Add(new FilePath(selectedItem.Text,
                        selectedItem.SubItems[1].Text));

                    totalBytes += new FileInfo(Path.Combine(directoryPath, filename)).Length;

                }

                FileCopy fileCopyDlg = new FileCopy(filesToCopy.ToArray(), 
                    copyFileFolderBrowserDialog.SelectedPath, totalBytes);

                if(fileCopyDlg.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Done");
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFiles();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshFileList();
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
            RefreshFileList();
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
            Close();
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = listView.SelectedItems[0].SubItems[1].Text;
            psi.Verb = "open";

            System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshFileList()
        {
            try
            {
                EnumerateFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace,
                    "Unholy Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFiles();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteFiles()
        {
            try
            {
                if (listView.SelectedItems.Count > 0)
                {
                    DialogResult dialogResult = MessageBox.Show(
                        "Delete selected files?",
                        "Confirm action",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        List<ListViewItem> pathsAndIndices = new List<ListViewItem>();

                        foreach (ListViewItem listViewItem in listView.SelectedItems)
                        {
                            pathsAndIndices.Add(listViewItem);
                        }

                        DeleteFilesDlg deleteFilesDlg = new DeleteFilesDlg(pathsAndIndices.ToArray());

                        if (deleteFilesDlg.ShowDialog() == DialogResult.OK)
                        {
                            for (int i = 0; i < pathsAndIndices.Count; i++)
                            {
                                listView.Items.Remove(pathsAndIndices[i]);
                                fileCount--;
                            }
                        }

                        UpdateStatusLabel();

                        // if there's nothing select, no point in using this.
                        // Disabled for sake of polish. 
                        if(listView.Items.Count == 0)
                        {
                            selectAllToolStripMenuItem.Enabled = false;
                        }
                    }
                }
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Unholy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Selected = true;
            }
        }

        private void recentDirectoriesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                selectedDirectory = e.ClickedItem.Text;

                UpdateRecentDirectoryMenuItems();

                EnumerateFiles();

                refreshToolStripMenuItem.Enabled = true;
                refreshToolStripButton.Enabled = true;
                selectAllToolStripMenuItem.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Unholy Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Arrays of strings are stored automatically as 
            // MultiString. Similarly, arrays of Byte are stored
            // automatically as Binary.
            string[] registryValues = (string[])Registry.GetValue(registryKey,
            "Directories", null);

            if (registryValues != null)
            {
                List<string> verifiedPaths = new List<string>();
          
                // verify we're not about to display garbage strings
                for(int i = 0; i < registryValues.Length; i++)
                {
                    if (Uri.IsWellFormedUriString(registryValues[i], UriKind.RelativeOrAbsolute))
                    {
                        verifiedPaths.Add(registryValues[i]);
                    }
                }

                recentDirectories.AddRange(verifiedPaths.ToArray());

                CreateRecentDirectoryMenuItems();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show(
                "Are you sure you wish to exit?",
                "Continue?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    string[] directories = recentDirectories.ToArray();
                    if (directories != null)
                    {
                        Registry.SetValue(registryKey, "Directories", directories);
                    }
                }
                else
                {
                    e.Cancel = true;
                    Activate();
                }
            }
            catch
            {
                throw;
            }
        }

        private void recentDirectoriesToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            
        }

        void CreateRecentDirectoryMenuItems()
        {
            recentDirectoriesToolStripMenuItem.DropDownItems.Clear();

            foreach (string s in recentDirectories)
            {
                recentDirectoriesToolStripMenuItem.DropDownItems.Add(s);
            }
        }
    }
}
