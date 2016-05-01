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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedSizedList<T> : IEnumerable<T>, ICollection<T>
    {
        //
        protected List<T> items;

        //
        protected int maxSize = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public FixedSizedList(int size)
        {
            items = new List<T>(size);
            maxSize = size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public FixedSizedList(T[] items) : this(items.Length)
        {
            try
            {
                this.items.AddRange(items);
                maxSize = items.Length;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator(); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if(item == null)
            {
                throw new ArgumentNullException();
            }

            if (items.Count == maxSize)
            {
                items.RemoveAt(maxSize - 1);
            }

            items.Insert(0, item);
        }

        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Contains(T item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                items.CopyTo(array, arrayIndex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int Count
        {
            get { return items.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove(T item)
        {
           return items.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void RemoveAt(int index)
        {
            try
            {
                items.RemoveAt(index);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return items.ToArray();
        }

        public void AddRange(T[] items)
        {
            try
            {
                this.items.AddRange(items);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual T this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }
    }
}
