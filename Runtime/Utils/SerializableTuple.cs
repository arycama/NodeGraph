using System;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public class SerializableTuple<T1, T2>
    {
        [SerializeField]
        private T1 item1 = default;

        [SerializeField]
        private T2 item2 = default;

        public T1 Item1 => item1;

        public T2 Item2 => item2;

        public SerializableTuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }
    }
}