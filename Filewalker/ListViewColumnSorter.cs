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
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filewalker
{
    /// <summary>
    /// Modified version of ListViewColumnSorter found at:
    /// https://support.microsoft.com/en-us/kb/319401 
    /// Used to sort the listview's columns.
    /// </summary>
    class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// the index specifying which column we're sorting
        /// <para/>0 = filename column
        /// <para/>1 = path (directory) column
        /// <para/>2 = size column
        /// <para/>3 = date column
        /// </summary>
        int column;

       
        /// <summary>
        /// Specifies whether the contents of the column
        /// is sorted in either ascending or descending order.
        /// </summary>
        SortOrder sortOrder = SortOrder.None;

        /// <summary>
        /// The comparer used when sorting the listview items by filename or path
        /// </summary>
        CaseInsensitiveComparer caseInsensitiveComparer = new CaseInsensitiveComparer();

        /// <summary>
        /// The comparer used when sorting the items in the list view by size
        /// </summary>
        FormattedFileSizeComparer fileSizeComparer = new FormattedFileSizeComparer();

        /// <summary>
        /// The comparer used when sorting items in the list view by creation date
        /// </summary>
        DateTimeComparer dateTimeComparer = new DateTimeComparer();

        /// <summary>
        /// Compares two objects and returns a value indicating whether 
        /// one is less than, equal to, or greater than the other. The method in which
        /// comparisons are made depends on the column selected.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>less than 0 if x is less than y, 0 if x equals y or 1 if x is greater than y.</returns>
        public int Compare(object x, object y)
        {
            int result;
            ListViewItem[] items = new ListViewItem[2];

            items[0] = (ListViewItem)x;
            items[1] = (ListViewItem)y;

            // name and directory columns
            if(column == 0 || column == 1)
            {
                result = caseInsensitiveComparer.Compare(
                items[0].SubItems[column].Text,
                items[1].SubItems[column].Text
                );
            }
            else if(column == 2)    // size column
            {
                result = fileSizeComparer.Compare(
                    items[0].SubItems[column].Text,
                    items[1].SubItems[column].Text
                    );
            }
            else // date column
            {
                result = dateTimeComparer.Compare(
                    items[0].SubItems[column].Text,
                    items[1].SubItems[column].Text
                );
            }

            if (sortOrder == SortOrder.Ascending)
            {
                return result;
            }
            else if(sortOrder == SortOrder.Descending)
            {
                return -result;
            }
            else
            {
                // 0 = the items are equal.
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number indicating which column is being sorted.
        /// </summary>
        public int SortColumn
        {
            get { return column; }
            set { column = value; }
        }

        /// <summary>
        /// Gets or sets the order in which the column is being sorted.
        /// </summary>
        public SortOrder SortingOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
    }
}
