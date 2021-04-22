using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Beem.Content {

    //TODO update to MCollections via NuGet

    public class SortedHashSet<K, V> {
        private Dictionary<K, V> elementsDictionary;
        private SortedSet<V> elementsSet;

        public SortedHashSet(IComparer<V> comparer) {
            elementsDictionary = new Dictionary<K, V>();
            elementsSet = new SortedSet<V>(comparer);
        }

        public void Add(K key, V value) {
            elementsDictionary[key] = value;
            elementsSet.Add(value);
        }

        public bool Contain(K key) {
            return elementsDictionary.ContainsKey(key);
        }

        public V GetByKey(K key) {
            if (!Contain(key))
                return default;

            return elementsDictionary[key];
        }

        public V GetByIndex(int index) {
            return elementsSet.ElementAt(index);
        }

        public void Remove(K key) {
            if (elementsDictionary.ContainsKey(key))
                return;

            elementsSet.Remove(elementsDictionary[key]);
            elementsDictionary.Remove(key);
        }

        public void Clear() {
            elementsDictionary.Clear();
            elementsSet.Clear();
        }

        public int Count() {
            return elementsSet.Count;
        }
    }
}