using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateCode
{
    public class ArrayOfX<T>
    {
        protected ArrayList elements;

        public ArrayOfX(int n)
        { elements = new ArrayList(n); }

        public ArrayOfX()
        { elements = new ArrayList(); }

        public int Add(T elem)
        { return elements.Add(elem); }

        public void Insert(int i, T elem)
        { elements.Insert(i, elem); }

        public int Count
        { get { return elements.Count; } }

        public void Remove(T elem)
        { elements.Remove(elem); }

        public void RemoveAll()
        { elements.RemoveRange(0, elements.Count); }

        public T this[int i]
        {
            get { Debug.Assert(i >= 0 && i < Count); return (T)elements[i]; }
            set { Debug.Assert(i >= 0 && i < Count); elements[i] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (T elem in elements)
                yield return elem;
        }

        #region Rotinas para impressão
        public override string ToString()
        {
            string str = "";

            if (elements.Count > 0)
            {
                int i;

                str += "\\{";
                for (i = 0; i < elements.Count - 1; i++)
                    str += this[i].ToString() + ", ";
                str += this[i].ToString() + "\\}";
            }

            return str;
        }
        #endregion
    }
}
