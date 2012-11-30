using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    internal static class KeyedCollectionHelper
    {
        public static TValue GetItemSecurely<TKey, TValue>(this KeyedCollection<TKey, TValue> collection, TKey key)
        {
            return collection != null && collection.Contains(key) ? collection[key] : default(TValue);
        }
    }
}
