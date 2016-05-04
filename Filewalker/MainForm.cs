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
        /// <summary>
        /// Used to sort the various columns in the list view.
        /// </summary>
        ListViewColumnSorter listViewColumnSorter = new ListViewColumnSorter();

        /// <summary>
        /// stores up to 10 recently opened directories; these are used to create
        /// the child menu items for the "recent directories" menu option.
        /// </summary>
        FixedSizedList<string> recentDirectories = new FixedSizedList<string>(10);

        /// <summary>
        /// stores the directory selected in the Folder browser dialog
        /// </summary>
        string selectedDirectory = null;

        /// <summary>
        /// The number of subdirectories present in the selected 
        /// directorry; this number also includes the selected directory. 
        /// </summary>
        int directoryCount = 0;

        /// <summary>
        /// the number of files present in the selected directory
        /// and its subdirectories. 
        /// </summary>
        int fileCount = 0;

        /// <summary>
        /// The registry key where recently opened directories will be stored.
        /// This is used to create at startup the submenu items for the "recent directories"
        /// menu item.
        /// </summary>
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
        /// Allows the user to select a directory and puts the application
        /// into a state where the files can be viewed and manipulated. 
        /// </summary>
        private void ChooseDirectory()
        {
            try
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedDirectory = folderBrowserDialog.SelectedPath;

                    // Add the selected directory to the list of recent directories 
                    UpdateRecentDirectoryMenuItems(selectedDirectory);

                    // Process all the files in the specified dir
                    EnumerateFiles();


                    refreshToolStripMenuItem.Enabled = true;
                    refreshToolStripButton.Enabled = true;

                    // There's no point in having a "select all" option
                    // if there's no files to select. 
                    if (listView.Items.Count > 0)
                        selectAllToolStripMenuItem.Enabled = true;
                    else
                        selectAllToolStripMenuItem.Enabled = false;
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

        /// <summary>
        /// Scans the recent directories list, removing items that match <i>dir</i>
        /// before adding it to the top of the list. 
        /// </summary>
        private void UpdateRecentDirectoryMenuItems(string dir)
        {
            // if this path has been opened before, we don't want
            // to display it twice. 
            for (int i = 0; i < recentDirectories.Count; i++)
            {
                if (recentDirectories[i] == dir)
                {
                    recentDirectories.RemoveAt(i);
                }
            }

            recentDirectories.Add(dir);

            CreateRecentDirectoryMenuItems();
        }

        /// <summary>
        /// Invokes the File Enumerator dialog. If the Dialog completes its task,
        /// the EnumerateFiles function adds the files to the list view and updates
        /// the statusbar with the number of files and directories contained within
        /// the selected directory.
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
        /// Returns a formatted string containing the absolute path of the file selected
        /// in the list view. 
        /// </summary>
        private string GetSelectedFilePath()
        {
            return Path.Combine(listView.SelectedItems[0].SubItems[1].Text, listView.SelectedItems[0].Text);
        }

        /// <summary>
        /// Updates the label on the toolstrip so the correct number of files and directories
        /// present in the list view is displayed. 
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

            //saveFileDialog.FileName = listView.SelectedItems[0].Text;
            if(copyFileFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // the total number of bytes that are to be copied. 
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

                // Actual copying of files begins here. As soon as the dialog is displayed,
                // the copying begins.
                FileCopy fileCopyDlg = new FileCopy(filesToCopy.ToArray(), 
                    copyFileFolderBrowserDialog.SelectedPath, totalBytes);

                // if the DialogResult is OK, the copying was performed successfully.
                // Any exceptions that are thrown during the copying process are handled
                // internally by the dialog.
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
            // open the selected directory in explorer
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = listView.SelectedItems[0].SubItems[1].Text;
            psi.Verb = "open";

            System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// Processes the specified directory again, effectively refreshing 
        /// the contents of the list view. This should never be called if the
        /// application hasn't been put into a state for manipulating files. Use
        /// <i>ChooseDirectory()</i> instead.
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
        /// Calling this function invokes the delete files dialog, 
        /// deleting the specified items from disk and removes them from the list view.
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
                        // Because apparently a ToArray() is too much to ask.
                        ListViewItem[] selectedItems = new ListViewItem[listView.SelectedItems.Count];
                        listView.SelectedItems.CopyTo(selectedItems, 0);

                        // Actual deleting of files begins once the dialog is shown. 
                        // Any exceptions thrown during the deletion process will be handled
                        // internally by the dialog. If an exception is caught, it's displayed
                        // and then treated as if the user has cancelled the task. 
                        DeleteFilesDlg deleteFilesDlg = new DeleteFilesDlg(selectedItems);

                        if (deleteFilesDlg.ShowDialog() == DialogResult.OK)
                        {
                            for (int i = 0; i < selectedItems.Length; i++)
                            {
                                listView.Items.Remove(selectedItems[i]);

                                // make sure the correct number of files displayed
                                // on the statusstrip.
                                fileCount--;
                            }
                        }

                        UpdateStatusLabel();

                        // if there's nothing to select, no point in having this
                        // enabled. 
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
                // "(none)" is just an dummy menu, used to tell the user
                // there are no items to display. Obviousy, we don't want
                // to try and open a folder with said name. 
                if (e.ClickedItem.Text != "(none)")
                {
                    selectedDirectory = e.ClickedItem.Text;

                    // move the selected directory to the top of the list
                    UpdateRecentDirectoryMenuItems(selectedDirectory);

                    // begin processing the directories contents and set
                    // the application into a state where the files can be 
                    // manipulated.
                    EnumerateFiles();

                    refreshToolStripMenuItem.Enabled = true;
                    refreshToolStripButton.Enabled = true;

                    if (listView.Items.Count > 0)
                        selectAllToolStripMenuItem.Enabled = true;
                    else
                        selectAllToolStripMenuItem.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Unholy Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // returns null if there's no keys
            string[] registryValues = (string[])Registry.GetValue(registryKey,
            "Directories", null);

            if (registryValues != null)
            {
                List<string> verifiedPaths = new List<string>();

                // verify we're not about to display garbage strings
                for (int i = 0; i < registryValues.Length; i++)
                {
                    if (IsValidPath(registryValues[i]))
                    {
                        verifiedPaths.Add(registryValues[i]);
                    }
                }

                recentDirectories.AddRange(verifiedPaths.ToArray());

                // Create menu items using the values we've read in from the registry
                CreateRecentDirectoryMenuItems();
            }
            else
            {
                // no items to add; create a menu item informing the user as such. 
                recentDirectoriesToolStripMenuItem.DropDownItems.Add("(none)");
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

        /// <summary>
        /// Creates the menu items that allows users to reopen previously opened directories
        /// </summary>
        void CreateRecentDirectoryMenuItems()
        {
            recentDirectoriesToolStripMenuItem.DropDownItems.Clear();

            foreach (string s in recentDirectories)
            {
                recentDirectoriesToolStripMenuItem.DropDownItems.Add(s);
            }

            if(recentDirectoriesToolStripMenuItem.DropDownItems.Count == 0)
            {
                recentDirectoriesToolStripMenuItem.DropDownItems.Add("(none)");
            }
        }

        /// <summary>
        /// Verifies whether a specified string is an absolute path or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns><b>true</b> if <i>path</i> contains a valid absolute path,
        /// else returns false.</returns>
        bool IsValidPath(string path)
        {
            try
            {
                // might seem a little wasteful, but DirectoryInfo throws
                // exceptions when a directory path contains illegal characters,
                // if path is null, the user doesn't have the required permission
                // to access the path or is too long. One line of code is easier.
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                
                // All paths must contain the drive they're located on; 
                // they're considered invalid if they don't (for our purposes anyway)
                if(Char.IsLetter(path[0]) && 
                   path[1] == ':' && 
                   path[2] == '\\')
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
