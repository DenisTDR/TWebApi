using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebGrease.Css.Extensions;

namespace customApiApp_3
{
    public static class Extensions
    {
        public static object MakeInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> source)
        {
            T buffer = default(T);
            bool buffered = false;

            foreach (T x in source)
            {
                if (buffered)
                    yield return buffer;

                buffer = x;
                buffered = true;
            }
        }

        public static NameValueCollection ToNameValueCollection<TKey, TValue>(
            this IDictionary<TKey, TValue> dict)
        {
            var nameValueCollection = new NameValueCollection();

            foreach (var kvp in dict)
            {
                string value = null;
                if (kvp.Value != null)
                    value = kvp.Value.ToString();

                nameValueCollection.Add(kvp.Key.ToString(), value);
            }

            return nameValueCollection;
        }

        public static TValue ValueAtIndex<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int index)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (index > dictionary.Count)
                throw new IndexOutOfRangeException("Index: " + index + " dictionary count:" + dictionary.Count);

            return dictionary[dictionary.Keys.ElementAt(index)];
        }

        public static Dictionary<TKey, TValue> AddDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            Dictionary<TKey, TValue> toAdd)
        {
            foreach (var kvp in toAdd)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }
            return dictionary;
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            col.AllKeys.ForEach(k => dict.Add(k, col[k]));
            return dict;
        }

        public static Dictionary<TKey, TValue> SkipD<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int howMany)
        {
            return dictionary.Skip(howMany).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}