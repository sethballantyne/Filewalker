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

namespace Filewalker
{
    /// <summary>
    /// Used to sort the filesizes in the list view. The strings FormattedFileSizeComparer
    /// sort must be formatted, ie: 1.2 MB  345 B. T
    /// </summary>
    /// <remarks>The strings that FormattedFileSizeComparer sort must be specifically formatted:
    /// 1.2 MB  345 B. The Comparer method attempts to read the measurement characters(MB, B, KB) 
    /// so they have to be present.
    /// </remarks>
    class FormattedFileSizeComparer : System.Collections.Generic.IComparer<string>
    {
        /// <summary>
        /// Used by the listview to sort the filesizes.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><para/>less than 0 if x is less than y
        /// <para/>0 if x equals y
        /// <para/>1 if x is greater than y</returns>
        /// <exception cref="System.FormatException"><i>x</i> or <i>y</i> does not contain
        /// a number in a valid format.</exception>
        /// <exception cref="System.OverflowException"><i>x</i> or <i>y</i> contains a value
        /// that is less than Double.MinValue or greater than Double.MaxValue.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The string contained in either
        /// <i>x</i> or <i>y</i> is too small to be a valid value.</exception>
        public int Compare(string x, string y)
        {
            try
            {
                //string xText = (string)x;
                //string yText = (string)y;

                string xMeasurement = x.Substring(x.Length - 2, 2).Trim();
                string yMeasurement = y.Substring(y.Length - 2, 2).Trim();

                // Test to see if the filesize of X is greater than Y
                if (xMeasurement == "MB")
                {
                    if (yMeasurement == "KB" || yMeasurement == "B")
                    {
                        // The filesize of x is greater than y
                        return 1;
                    }
                }
                else if (xMeasurement == "KB" && yMeasurement == "B")
                {
                    return 1;
                }

                // The filesize of X is not greater than Y; test to see if Y is greater
                // than X.
                if (yMeasurement == "MB")
                {
                    if (xMeasurement == "KB" || xMeasurement == "B")
                    {
                        return -1;
                    }
                }
                else if (yMeasurement == "KB" && xMeasurement == "B")
                {
                    return -1;
                }

                // Both X and Y are using the same measurement (they're both in megabytes, or 
                // kilobytes or bytes). 
                // determine which one is greater than the other.
                char[] seperator = new char[1] { ' ' };
                double xSize = Convert.ToDouble(x.Split(' ')[0]);
                double ySize = Convert.ToDouble(y.Split(' ')[0]);

                if (xSize > ySize)
                {
                    return 1;
                }
                else if (xSize < ySize)
                {
                    return -1;
                }

                return 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
