namespace HashTable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    public class HashTable<TKey, TValue> : IEnumerable<KeyValue<TKey, TValue>>
    {
        public const int InitialCapacity = 16;
        private const float LoadFactor = 0.75f;

        private LinkedList<KeyValue<TKey, TValue>>[] slots;

        public int Count { get; private set; }

        public int Capacity => this.slots.Length;

        public HashTable() : this(InitialCapacity) {  }

        public HashTable(int capacity = InitialCapacity)
        {
            this.slots = new LinkedList<KeyValue<TKey, TValue>>[capacity];
            this.Count = 0;
        }

        public void Add(TKey key, TValue value)
        {
            this.GrowIfNeeded();
            int slotIndex = this.FindSlotNumber(key);
            if (this.slots[slotIndex] == null)
            {
                this.slots[slotIndex] = new LinkedList<KeyValue<TKey, TValue>>();
            }

            foreach (var element in this.slots[slotIndex])
            {
                if (element.Key.Equals(key))
                {
                    throw new ArgumentException($"Key already exists: {key}");
                }
            }

            var newElement = new KeyValue<TKey, TValue>(key, value);
            this.slots[slotIndex].AddLast(newElement);
            this.Count++;
        }

        public bool AddOrReplace(TKey key, TValue value)
        {
            try
            {
                this.Add(key, value);
            }
            catch (ArgumentException)
            {
                int index = this.FindSlotNumber(key);

                var kvp = this.slots[index].First(kv => kv.Key.Equals(key));
                kvp.Value = value;
                return true;
            }

            return false;
        }

        public TValue Get(TKey key)
        {
            var element = this.Find(key);
            if (element is null)
            {
                throw new KeyNotFoundException();
            }
            return element.Value;
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.Get(key);
            }
            set
            {
                this.AddOrReplace(key, value);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var element = this.Find(key);

            if(element != null)
            {
                value = element.Value;
                return true;
            }

            value = default;
            return false;
        }

        public KeyValue<TKey, TValue> Find(TKey key)
        {
            int slotNumber = this.FindSlotNumber(key);
            var elements = this.slots[slotNumber];
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    if (element.Key.Equals(key))
                    {
                        return element;
                    }   
                }
            }

            return null;
        }

        public bool ContainsKey(TKey key)
        {
            return this.Find(key) != null;
        }

        public bool Remove(TKey key)
        {
            int slotNumber = this.FindSlotNumber(key);
            var elements = this.slots[slotNumber];
            if (elements != null)
            {
                var currentElement = elements.First;
                while (currentElement != null)
                {
                    if (currentElement.Value.Key.Equals(key))
                    {
                        elements.Remove(currentElement);
                        this.Count--;
                        return true;
                    }
                    currentElement = currentElement.Next;
                }
            }

            return false;
        }

        public void Clear()
        {
            this.slots = new LinkedList<KeyValue<TKey, TValue>>[InitialCapacity];
            this.Count = 0;
        }

        public IEnumerable<TKey> Keys => this.Select(kvp => kvp.Key);

        public IEnumerable<TValue> Values => this.Select(kvp => kvp.Value);

        public IEnumerator<KeyValue<TKey, TValue>> GetEnumerator()
        {
            foreach (var elements in this.slots)
            {
                if (elements != null)
                {
                    foreach (var element in elements)
                    {
                        yield return element;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        //helper methods
        private void GrowIfNeeded()
        {
            if ((float)(this.Count + 1) / this.Capacity > LoadFactor)
            {
                this.Grow();
            }
        }

        private void Grow()
        {
            var newHashTable = new HashTable<TKey, TValue>(2 * this.Capacity);
            foreach (var element in this)
            {
                newHashTable.Add(element.Key, element.Value);
            }

            this.slots = newHashTable.slots;
            this.Count = newHashTable.Count;
        }

        private int FindSlotNumber(TKey key)
        {
            var slotNumber = Math.Abs(key.GetHashCode()) % this.slots.Length;
            return slotNumber;
        }
    }
}
