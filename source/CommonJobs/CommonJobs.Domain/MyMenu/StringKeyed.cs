using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class StringKeyedCollection<T> : KeyedCollection<string, T>
        where T : IKeyText
    {
        protected override string GetKeyForItem(T item)
        {
            return item.Key;
        }
    }

    public interface IKeyText
    {
        string Key { get; }
        string Text { get; }
    }
}
