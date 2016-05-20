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
    /// Represents a list of fixed size. When the max number of items is added
    /// to the list, the next item to be added will be inserted to the top of the list,
    /// while the bottom item is removed. The list never exceeds the specified number of items.
    /// </summary>
    /// <typeparam name="T">The type of element contained in the list.</typeparam>
    public class FixedSizedList<T> : IEnumerable<T>, ICollection<T>
    {
        protected List<T> items;

        /// <summary>
        /// The maximum number of items that can be stored in the list
        /// </summary>
        protected int maxSize = 0;

        /// <summary>
        /// Initialises a new instance of the list, setting the maximum number of items it will hold
        /// to <i>size</i>.
        /// </summary>
        /// <param name="size">The maximum number of elements the list will contain.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><i>size</i> is less than 0.</exception>
        public FixedSizedList(int size)
        {
            try
            {
                items = new List<T>(size);
                maxSize = size;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Initialises the list with the specified items. The maximum number of items
        /// the list can contain is set to the length of <i>items</i>.
        /// </summary>
        /// <param name="items">The items to fill the list with.</param>
        /// <exception cref="System.ArgumentNullException">An element in the array evaluates to <b>null</b>,
        /// or <i>items</i> itself is <b>null</b>.</exception>
        public FixedSizedList(T[] items) : this(items.Length)
        {
            try
            {
                AddRange(items);
                maxSize = items.Length;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator(); 
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an object to the start of the list.
        /// </summary>
        /// <param name="item">The object to add to the list.</param>
        /// <exception cref="System.ArgumentNullException"><i>item</i> is <b>null</b>.</exception>
        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (items.Count == maxSize)
            {
                items.RemoveAt(maxSize - 1);
            }

            items.Insert(0, item);
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// Determines whether the list contains a specified item.
        /// </summary>
        /// <param name="item">the item to search for.</param>
        /// <returns>true if the item is present in the list, otherwise false.</returns>
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Copies the list to an array, starting at the specified element
        /// </summary>
        /// <param name="array">The array that is the destination of the elements copied from the list</param>
        /// <param name="arrayIndex">The position in the array to start copying from.</param>
        /// <exception cref="System.ArgumentNullException"><i>array</i> is <b>null</b>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><i>arrayIndex</i> is less than 0.</exception>
        /// <exception cref="System.ArgumentException">Not enough elements in <i>array</i> to make the 
        /// copy.</exception>
        public void CopyTo(T[] array, int arrayIndex)
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
        /// Returns the number of items in the list.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Removes the first occurance of the specified object from the list.
        /// </summary>
        /// <param name="item">the item to remove.</param>
        /// <returns>true if the item is successfully removed, otherwise. False is 
        /// also returned if the item wasn't found.</returns>
        public bool Remove(T item)
        {
           return items.Remove(item);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.ArgumentOutOfRangeException">index refers to an element that doesn't exist</exception>
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
        /// Copies the elements of the list to a new array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return items.ToArray();
        }

        /// <summary>
        /// Adds the specified array of items to the list
        /// </summary>
        /// <param name="items">The array of items to add to the list. Items are addded at the start
        /// of the list, not the end.</param>
        /// <exception cref="System.ArgumentNullException">An item in the array contains a null element,
        /// or null has been passed as an argument.</exception>
        /// <remarks>items </remarks>
        public void AddRange(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            try
            {
                
                // not the most efficient way
                for (int i = 0; i < items.Length; i++)
                {
                    Add(items[i]);
                }
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
        /// <returns>The item at the specified index</returns>
        /// <exception cref="System.IndexOutOfRangeException"><i>index</i> refers to an element that doesn't exist.</exception>
        public T this[int index]
        {
            get 
            {
                if (index < 0 || index > maxSize)
                    throw new IndexOutOfRangeException();

                return items[index]; 
            }
            set 
            {
                if (index < 0 || index > maxSize)
                {
                    throw new IndexOutOfRangeException();
                }

                items[index] = value; 
            }
        }
    }
}
