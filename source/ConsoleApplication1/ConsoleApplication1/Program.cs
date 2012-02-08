using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new TableCountryMappingCollection();
            a.Add(new TableCountryMapping());
            a.Add(new TableCountryMapping());
            foreach (var i in a)
                Console.WriteLine(i);
        }
    }

    public class TableCountryMappingCollection : ConfigurationElementCollection, IList<TableCountryMapping>
    {
        
        public int IndexOf(TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public TableCountryMapping this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(TableCountryMapping item)
        {
            base.BaseAdd((ConfigurationElement)item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TableCountryMapping[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public new bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            throw new NotImplementedException();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return 0;
        }

        int IList<TableCountryMapping>.IndexOf(TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        void IList<TableCountryMapping>.Insert(int index, TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        void IList<TableCountryMapping>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        TableCountryMapping IList<TableCountryMapping>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<TableCountryMapping>.Add(TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TableCountryMapping>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<TableCountryMapping>.Contains(TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TableCountryMapping>.CopyTo(TableCountryMapping[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<TableCountryMapping>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<TableCountryMapping>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<TableCountryMapping>.Remove(TableCountryMapping item)
        {
            throw new NotImplementedException();
        }

        public new IEnumerator<TableCountryMapping> GetEnumerator()
        {
            foreach (TableCountryMapping item in (IEnumerable)this)
                yield return item;
        }
        /*
        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }*/
    }
}
