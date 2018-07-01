using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Helpers
{

    // The SortedDictionary class implements a generic sorted list of keys 
    // and values. Entries in a sorted list are sorted by their keys and 
    // are accessible both by key and by index. The keys of a sorted dictionary
    // can be ordered either according to a specific IComparer 
    // implementation given when the sorted dictionary is instantiated, or 
    // according to the IComparable implementation provided by the keys 
    // themselves. In either case, a sorted dictionary does not allow entries
    // with duplicate or null keys.
    // 
    // A sorted list internally maintains two arrays that store the keys and
    // values of the entries. The capacity of a sorted list is the allocated
    // length of these internal arrays. As elements are added to a sorted list, the
    // capacity of the sorted list is automatically increased as required by
    // reallocating the internal arrays.  The capacity is never automatically 
    // decreased, but users can call either TrimExcess or 
    // Capacity explicitly.
    // 
    // The GetKeyList and GetValueList methods of a sorted list
    // provides access to the keys and values of the sorted list in the form of
    // List implementations. The List objects returned by these
    // methods are aliases for the underlying sorted list, so modifications
    // made to those lists are directly reflected in the sorted list, and vice
    // versa.
    // 
    // The SortedList class provides a convenient way to create a sorted
    // copy of another dictionary, such as a Hashtable. For example:
    // 
    // Hashtable h = new Hashtable();
    // h.Add(...);
    // h.Add(...);
    // ...
    // SortedList s = new SortedList(h);
    // 
    // The last line above creates a sorted list that contains a copy of the keys
    // and values stored in the hashtable. In this particular example, the keys
    // will be ordered according to the IComparable interface, which they
    // all must implement. To impose a different ordering, SortedList also
    // has a constructor that allows a specific IComparer implementation to
    // be specified.
    // 
    [DebuggerDisplay("Count = {Count}")]
#if !FEATURE_NETCORE
    [Serializable()]
#endif
    [System.Runtime.InteropServices.ComVisible(false)]
    public class ConcurrentSortedList<TKey, TValue> :
        IDictionary<TKey, TValue>, System.Collections.IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        private TKey[] _keys;
        private TValue[] _values;
        private int _size;
        private IComparer<TKey> _comparer;
        private KeyList _keyList;
        private ValueList _valueList;
#if !FEATURE_NETCORE
        [NonSerialized]
#endif
        private object _syncRoot = new object();

        static TKey[] emptyKeys = new TKey[0];
        static TValue[] emptyValues = new TValue[0];

        private const int _defaultCapacity = 4;

        #region Constructors
        public ConcurrentSortedList()
        {
            _keys = emptyKeys;
            _values = emptyValues;
            _size = 0;
            _comparer = Comparer<TKey>.Default;
            _keyList = new KeyList(this);
            _valueList = new ValueList(this);
        }

        public ConcurrentSortedList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be a non negative number.");

            _keys = new TKey[capacity];
            _values = new TValue[capacity];
            _comparer = Comparer<TKey>.Default;
            _keyList = new KeyList(this);
            _valueList = new ValueList(this);
        }

        public ConcurrentSortedList(IComparer<TKey> comparer)
                : this()
        {
            if (comparer != null)
            {
                this._comparer = comparer;
            }
        }

        public ConcurrentSortedList(int capacity, IComparer<TKey> comparer)
                : this(comparer)
        {
            Capacity = capacity;
        }

        public ConcurrentSortedList(IDictionary<TKey, TValue> dictionary)
                : this(dictionary, null)
        {
        }

        public ConcurrentSortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
                : this((dictionary != null ? dictionary.Count : 0), comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            dictionary.Keys.CopyTo(_keys, 0);
            dictionary.Values.CopyTo(_values, 0);
            Array.Sort<TKey, TValue>(_keys, _values, comparer);
            _size = dictionary.Count;
        }
        #endregion

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int i;

            lock (_syncRoot)
            {
                i = Array.BinarySearch<TKey>(_keys, 0, _size, key, _comparer);

                if (i >= 0)
                    throw new ArgumentException("Adding duplicate value");

                Insert(~i, key, value);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            lock (_syncRoot)
            {
                int index = IndexOfKeyPrivat(keyValuePair.Key);

                if (index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], keyValuePair.Value))
                {
                    return true;
                }
            }
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index;

            lock (_syncRoot)
            {
                index = IndexOfKeyPrivat(keyValuePair.Key);

                if (index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], keyValuePair.Value))
                {
                    RemoveAtPrivate(index);
                    return true;
                }
            }

            return false;
        }

        public int Capacity
        {
            get
            {
                lock (_syncRoot)
                    return _keys.Length;
            }
            set
            {
                lock (_syncRoot)
                    ChangeCapacity(value);
            }
        }

        private void ChangeCapacity(int newCapacity)
        {
            if (newCapacity != _keys.Length)
            {
                if (newCapacity < _size)
                {
                    throw new ArgumentOutOfRangeException("value", newCapacity, "Small capacity");
                }

                if (newCapacity > 0)
                {
                    TKey[] newKeys = new TKey[newCapacity];
                    TValue[] newValues = new TValue[newCapacity];
                    if (_size > 0)
                    {
                        Array.Copy(_keys, 0, newKeys, 0, _size);
                        Array.Copy(_values, 0, newValues, 0, _size);
                    }
                    _keys = newKeys;
                    _values = newValues;
                }
                else
                {
                    _keys = emptyKeys;
                    _values = emptyValues;
                }
            }
        }

        public IComparer<TKey> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        void System.Collections.IDictionary.Add(Object key, Object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null && !(default(TValue) == null))
                throw new ArgumentNullException("value");

            try
            {
                TKey tempKey = (TKey)key;

                try
                {
                    Add(tempKey, (TValue)value);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Wrong type of argument", "value");
                }
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Wrong type of argument", "key");
            }
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                    return _size;
            }
        }

        #region Keys
        public IList<TKey> Keys
        {
            get
            {
                return _keyList;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return _keyList;
            }
        }

        System.Collections.ICollection System.Collections.IDictionary.Keys
        {
            get
            {
                return _keyList;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return _keyList;
            }
        }
        #endregion

        #region Values
        public IList<TValue> Values
        {
            get
            {
                return _valueList;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return _valueList;
            }
        }

        System.Collections.ICollection System.Collections.IDictionary.Values
        {
            get
            {
                return _valueList;
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return _valueList;
            }
        }
        #endregion

        #region IsReadOnly
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool System.Collections.IDictionary.IsReadOnly
        {
            get { return false; }
        }
        #endregion

        bool System.Collections.IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return true; }
        }

        Object System.Collections.ICollection.SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
                ClearPrivate();
        }

        private void ClearPrivate()
        {
            // clear does not change the capacity
            // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            Array.Clear(_keys, 0, _size);
            Array.Clear(_values, 0, _size);
            _size = 0;
        }

        bool System.Collections.IDictionary.Contains(Object key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key);
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            lock (_syncRoot)
                return IndexOfKeyPrivat(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            lock (_syncRoot)
                return IndexOfValuePrivat(value) >= 0;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Array index must be non negativ number, and must not be greater than the length of array.");
            }

            lock (_syncRoot)
                CopyToPrivate(array, arrayIndex);
        }

        void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("multidimention array not supported.", "array");
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException("Non zero lower bound.", "array");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Array index must be non negativ number, and must not be greater than the length of array.");
            }

            lock (_syncRoot)
                CopyToPrivate(array, arrayIndex);
        }

        private void CopyToPrivate(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Offset plus count is larger than array");
            }

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }

        private void CopyToPrivate(Array array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Offset plus count is larger than array");
            }

            KeyValuePair<TKey, TValue>[] keyValuePairArray = array as KeyValuePair<TKey, TValue>[];
            if (keyValuePairArray != null)
            {
                    for (int i = 0; i < Count; i++)
                    {
                        keyValuePairArray[i + arrayIndex] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
                    }
            }
            else
            {
                object[] objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException("invalid array type", "array");
                }

                try
                {
                    for (int i = 0; i < Count; i++)
                    {
                        objects[i + arrayIndex] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type");
                }
            }
        }

        private const int MaxArrayLength = 0X7FEFFFFF;

        private void EnsureCapacity(int min)
        {
            int newCapacity = _keys.Length == 0 ? _defaultCapacity : _keys.Length * 2;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > MaxArrayLength) newCapacity = MaxArrayLength;
            if (newCapacity < min) newCapacity = min;
            ChangeCapacity(newCapacity);
        }

        private TValue GetByIndex(int index)
        {
            lock (_syncRoot)
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException("index", "Index must be non negative number, and must not be greater than the size of the list.");

                return _values[index];
            }
        }

        #region GetEnumerator
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.DictEntry);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }
        #endregion

        private TKey GetKey(int index)
        {
            lock (_syncRoot)
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException("index", "Index must be non negative number, and must not be greater than the size of the list.");

                return _keys[index];
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_syncRoot)
                {
                    int i = IndexOfKeyPrivat(key);
                    if (i >= 0)
                        return _values[i];
                }

                return default(TValue);
            }
            set
            {
                if (((Object)key) == null)
                    throw new ArgumentNullException("key");

                lock (_syncRoot)
                {
                    int i = Array.BinarySearch<TKey>(_keys, 0, _size, key, _comparer);
                    if (i >= 0)
                        _values[i] = value;
                    else
                        Insert(~i, key, value);
                }
            }
        }

        Object System.Collections.IDictionary.this[Object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                    return this[(TKey)key];

                return null;
            }
            set
            {
                if (!IsCompatibleKey(key))
                    throw new ArgumentNullException("key");

                try
                {
                    TKey tempKey = (TKey)key;
                    try
                    {
                        this[tempKey] = (TValue)value;
                    }
                    catch (InvalidCastException)
                    {
                        throw new ArgumentException("Wrong value type", "value");
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Wrong key type", "key");
                }
            }
        }

        public int IndexOfKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (_syncRoot)
                return IndexOfKeyPrivat(key);
        }

        private int IndexOfKeyPrivat(TKey key)
        {
            int ret = Array.BinarySearch<TKey>(_keys, 0, _size, key, _comparer);

            return ret >= 0 ? ret : -1;
        }

        public int IndexOfValue(TValue value)
        {
            lock (_syncRoot)
                return IndexOfValuePrivat(value);
        }

        private int IndexOfValuePrivat(TValue value)
        {
            return Array.IndexOf(_values, value, 0, _size);
        }

        private void Insert(int index, TKey key, TValue value)
        {
            if (_size == _keys.Length) EnsureCapacity(_size + 1);
            if (index < _size)
            {
                Array.Copy(_keys, index, _keys, index + 1, _size - index);
                Array.Copy(_values, index, _values, index + 1, _size - index);
            }
            _keys[index] = key;
            _values[index] = value;
            _size++;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_syncRoot)
            {
                int i = IndexOfKey(key);
                if (i >= 0)
                {
                    value = _values[i];
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        public void RemoveAt(int index)
        {
            lock (_syncRoot)
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException("index", "Index must be non negative number, and must not be greater than the size of the array");

                RemoveAtPrivate(index);
            }
        }

        private void RemoveAtPrivate(int index)
        {
            _size--;
            if (index < _size)
            {
                Array.Copy(_keys, index + 1, _keys, index, _size - index);
                Array.Copy(_values, index + 1, _values, index, _size - index);
            }
            _keys[_size] = default(TKey);
            _values[_size] = default(TValue);
        }

        public bool Remove(TKey key)
        {
            int i;

            lock (_syncRoot)
            {
                i = IndexOfKeyPrivat(key);
                if (i >= 0)
                    RemoveAtPrivate(i);
            }

            return i >= 0;
        }

        void System.Collections.IDictionary.Remove(Object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        public void TrimExcess()
        {
            lock (_syncRoot)
            {
                int threshold = (int)(((double)_keys.Length) * 0.9);
                if (_size < threshold)
                {
                    ChangeCapacity(_size);
                }
            }
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return (key is TKey);
        }


#if !FEATURE_NETCORE
        [Serializable()]
#endif
        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, System.Collections.IDictionaryEnumerator
        {
            private ConcurrentSortedList<TKey, TValue> _sortedList;
            private TKey _key;
            private TValue _value;
            private int _index;
            private int _getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            internal Enumerator(ConcurrentSortedList<TKey, TValue> sortedList, int getEnumeratorRetType)
            {
                _sortedList = sortedList;
                _index = 0;
                _getEnumeratorRetType = getEnumeratorRetType;
                _key = default(TKey);
                _value = default(TValue);
            }

            public void Dispose()
            {
                _index = 0;
                _key = default(TKey);
                _value = default(TValue);
            }

            public bool MoveNext()
            {
                lock(_sortedList._syncRoot)
                {
                    if (_index < _sortedList.Count)
                    {
                        _key = _sortedList._keys[_index];
                        _value = _sortedList._values[_index];
                        _index++;
                        return true;
                    }

                    _index = _sortedList.Count + 1;
                }

                _key = default(TKey);
                _value = default(TValue);
                return false;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>(_key, _value);
                }
            }

            Object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (_getEnumeratorRetType == DictEntry)
                    {
                        return new System.Collections.DictionaryEntry(_key, _value);
                    }
                    else
                    {
                        return new KeyValuePair<TKey, TValue>(_key, _value);
                    }
                }
            }

            public object Key
            {
                get
                {
                    return _key;
                }
            }

            public object Value
            {
                get
                {
                    return _value;
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry(_key, _value);
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                _index = 0;
                _key = default(TKey);
                _value = default(TValue);
            }
        }

#if !FEATURE_NETCORE
        [Serializable()]
#endif
        private sealed class SortedListKeyEnumerator : IEnumerator<TKey>, System.Collections.IEnumerator
        {
            private ConcurrentSortedList<TKey, TValue> _sortedList;
            private int _index;
            private TKey _currentKey;

            internal SortedListKeyEnumerator(ConcurrentSortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
            }

            public void Dispose()
            {
                _index = 0;
                _currentKey = default(TKey);
            }

            public bool MoveNext()
            {
                lock (_sortedList._syncRoot)
                {
                    if (_index < _sortedList.Count)
                    {
                        _currentKey = _sortedList._keys[_index++];
                        return true;
                    }

                    _index = _sortedList.Count + 1;
                }

                _currentKey = default(TKey);
                return false;
            }

            public TKey Current
            {
                get
                {
                    return _currentKey;
                }
            }

            Object System.Collections.IEnumerator.Current
            {
                get
                {
                    return _currentKey;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                _index = 0;
                _currentKey = default(TKey);
            }
        }

#if !FEATURE_NETCORE
        [Serializable()]
#endif
        private sealed class SortedListValueEnumerator : IEnumerator<TValue>, System.Collections.IEnumerator
        {
            private ConcurrentSortedList<TKey, TValue> _sortedList;
            private int _index;
            private TValue _currentValue;

            internal SortedListValueEnumerator(ConcurrentSortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
            }

            public void Dispose()
            {
                _index = 0;
                _currentValue = default(TValue);
            }

            public bool MoveNext()
            {
                lock (_sortedList._syncRoot)
                {
                    if (_index < _sortedList.Count)
                    {
                        _currentValue = _sortedList._values[_index++];

                        return true;
                    }

                    _index = _sortedList.Count + 1;
                }

                _currentValue = default(TValue);

                return false;
            }

            public TValue Current
            {
                get
                {
                    return _currentValue;
                }
            }

            Object System.Collections.IEnumerator.Current
            {
                get
                {
                    return _currentValue;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                _index = 0;
                _currentValue = default(TValue);
            }
        }

        [DebuggerDisplay("Count = {Count}")]
#if !FEATURE_NETCORE
        [Serializable()]
#endif
        private sealed class KeyList : IList<TKey>, System.Collections.ICollection
        {
            private ConcurrentSortedList<TKey, TValue> _dict;

            internal KeyList(ConcurrentSortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public int Count
            {
                get
                {
                    lock (_dict._syncRoot)
                        return _dict._size;
                }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public object SyncRoot
            {
                get
                {
                    return _dict._syncRoot;
                }
            }

            public bool IsSynchronized
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public void Add(TKey key)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TKey key)
            {
                return _dict.ContainsKey(key);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                lock (_dict._syncRoot)
                    Array.Copy(_dict._keys, 0, array, arrayIndex, _dict.Count);
            }

            public void Insert(int index, TKey value)
            {
                throw new NotSupportedException();
            }

            public TKey this[int index]
            {
                get
                {
                    return _dict.GetKey(index);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new SortedListKeyEnumerator(_dict);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new SortedListKeyEnumerator(_dict);
            }

            public int IndexOf(TKey key)
            {
                if (((Object)key) == null)
                    throw new ArgumentNullException("key");

                int i;

                lock (_dict._syncRoot)
                {
                    i = Array.BinarySearch<TKey>(_dict._keys, 0, _dict.Count, key, _dict._comparer);

                    if (i >= 0) return i;
                }

                return -1;
            }

            public bool Remove(TKey key)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public void CopyTo(Array array, int index)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException("Multidimentional array is not supported.", "array");

                lock (_dict._syncRoot)
                    Array.Copy(_dict._values, 0, array, index, _dict.Count);
            }
        }

        [DebuggerDisplay("Count = {Count}")]
#if !FEATURE_NETCORE
        [Serializable()]
#endif
        private sealed class ValueList : IList<TValue>, System.Collections.ICollection
        {
            private ConcurrentSortedList<TKey, TValue> _dict;

            internal ValueList(ConcurrentSortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public int Count
            {
                get
                {
                    lock (_dict._syncRoot)
                        return _dict._size;
                }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            bool System.Collections.ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object System.Collections.ICollection.SyncRoot
            {
                get { return _dict._syncRoot; }
            }

            public void Add(TValue key)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TValue value)
            {
                return _dict.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                lock (_dict._syncRoot)
                    Array.Copy(_dict._values, 0, array, arrayIndex, _dict.Count);
            }

            void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException("Multidimentional array is not supported.", "array");

                lock (_dict._syncRoot)
                    Array.Copy(_dict._values, 0, array, arrayIndex, _dict.Count);
            }

            public void Insert(int index, TValue value)
            {
                throw new NotSupportedException();
            }

            public TValue this[int index]
            {
                get
                {
                    return _dict.GetByIndex(index);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new SortedListValueEnumerator(_dict);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new SortedListValueEnumerator(_dict);
            }

            public int IndexOf(TValue value)
            {
                lock (_dict._syncRoot)
                    return Array.IndexOf(_dict._values, value, 0, _dict.Count);
            }

            public bool Remove(TValue value)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
        }
    }
}
