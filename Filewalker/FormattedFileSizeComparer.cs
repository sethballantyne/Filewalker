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
