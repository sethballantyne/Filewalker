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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewalker
{
    /// <summary>
    /// 
    /// </summary>
    class FormattedFileSizeComparer : System.Collections.IComparer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            string xText = (string)x;
            string yText = (string)y;

            string xMeasurement = xText.Substring(xText.Length - 2, 2).Trim();
            string yMeasurement = yText.Substring(yText.Length - 2, 2).Trim();

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
            if(yMeasurement == "MB")
            {
                if (xMeasurement == "KB" || xMeasurement == "B")
                {
                    return -1;
                }
            }
            else if(yMeasurement == "KB" && xMeasurement == "B")
            {
                return -1;
            }

            // Both X and Y are using the same measurement (they're both in megabytes, or 
            // kilobytes or bytes). 
            // determine which one is greater than the other.
            char[] seperator = new char[1] { ' ' };
            double xSize = Convert.ToDouble(xText.Split(' ')[0]);
            double ySize = Convert.ToDouble(yText.Split(' ')[0]);

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
    }
}