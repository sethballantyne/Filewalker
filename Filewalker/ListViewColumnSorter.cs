﻿/*Copyright (c) 2016 Seth Ballantyne <seth.ballantyne@gmail.com>
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
    /// </summary>
    class ListViewColumnSorter : IComparer
    {
        //
        int column;

        //
        SortOrder sortOrder = SortOrder.None;

        //
        CaseInsensitiveComparer caseInsensitiveComparer = new CaseInsensitiveComparer();

        //
        FormattedFileSizeComparer fileSizeComparer = new FormattedFileSizeComparer();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            int result;
            ListViewItem[] items = new ListViewItem[2];
            items[0] = (ListViewItem)x;
            items[1] = (ListViewItem)y;

            if(column != 2)
            {
                result = caseInsensitiveComparer.Compare(
                items[0].SubItems[column].Text,
                items[1].SubItems[column].Text
                );
            }
            else
            {
                result = fileSizeComparer.Compare(
                    items[0].SubItems[column].Text,
                    items[1].SubItems[column].Text
                    );
                //result = caseInsensitiveComparer.Compare(
                //Convert.ToInt64(items[0].SubItems[column].Text),
                //Convert.ToInt64(items[1].SubItems[column].Text)
                //);
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
        /// 
        /// </summary>
        public int SortColumn
        {
            get { return column; }
            set { column = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SortOrder SortingOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
    }
}
