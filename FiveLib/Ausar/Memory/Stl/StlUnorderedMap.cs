﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FiveLib.Common;
using FiveLib.IO;
using FiveLib.Memory;

namespace FiveLib.Ausar.Memory.Stl
{
    /// <summary>
    /// Memory interface for std::unordered_map.
    /// </summary>
    /// <typeparam name="TKey">The key structure.</typeparam>
    /// <typeparam name="TValue">The value structure.</typeparam>
    /// <typeparam name="THash">The hasher object.</typeparam>
    public class StlUnorderedMap<TKey, TValue, THash> : IBinarySerializable, IFixedSize
        where TKey: IBinarySerializable, IFixedSize, new()
        where TValue: IBinarySerializable, IFixedSize, new()
        where THash: IStlHash<TKey>, new()
    {
        private readonly THash _hash = new THash();

        public float MaxBucketSize;
        public StlList<StlPair<TKey, TValue>> Elements = new StlList<StlPair<TKey, TValue>>();

        // Vector of (begin, end - 1) iterator pairs
        public StlVector<StlPair<ListIterator, ListIterator>> Buckets = new StlVector<StlPair<ListIterator, ListIterator>>();

        public ulong KeyMask;
        public long MaxKey;

        public ulong Count => Elements.Count;

        public bool TryGetValue(TKey key, BinaryReader reader, out TValue value)
        {
            value = default(TValue);
            var keyHash = _hash.Hash(key);
            var bucketIndex = keyHash & KeyMask;
            var bucket = Buckets.Get(bucketIndex, reader);
            var begin = bucket.First.Pointer;
            if (begin == Elements.End)
                return false;
            var last = bucket.Second.Pointer;
            var end = last.Get(reader).Next;
            var currentNode = begin;
            while (currentNode != end)
            {
                var nodeData = currentNode.Get(reader);
                if (nodeData.Data.First.Equals(key))
                {
                    value = nodeData.Data.Second;
                    return true;
                }
                currentNode = nodeData.Next;
            }
            return false;
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(BinaryReader reader)
        {
            return Elements.Enumerate(reader).Select(n => new KeyValuePair<TKey, TValue>(n.Data.First, n.Data.Second));
        }

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref MaxBucketSize);
            s.Padding(4);
            s.Object(ref Elements);
            s.Object(ref Buckets);
            s.Value(ref KeyMask);
            s.Value(ref MaxKey);
        }

        public ulong GetStructSize() => 0x8 + Elements.GetStructSize() + Buckets.GetStructSize() + 0x10;

        public ulong GetStructAlignment() => 0x8;

        public class ListIterator : IBinarySerializable, IFixedSize
        {
            public Pointer64<StlList<StlPair<TKey, TValue>>.Node> Pointer;

            public void Serialize(BinarySerializer s)
            {
                s.Value(ref Pointer);
            }

            public ulong GetStructSize() => Pointer.GetStructSize();

            public ulong GetStructAlignment() => Pointer.GetStructAlignment();
        }
    }
}
