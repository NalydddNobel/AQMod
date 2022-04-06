using System.Collections.Generic;

namespace Aequus.Effects
{
    public sealed class DrawIndexCache
    {
        public int Count => List.Count;

        internal readonly List<int> List;
        public bool renderingNow;

        public DrawIndexCache()
        {
            List = new List<int>();
            renderingNow = false;
        }

        public int Index(int i)
        {
            return List[i];
        }

        public void Add(int value)
        {
            List.Add(value);
        }

        public void Clear()
        {
            renderingNow = false;
            List.Clear();
        }
    }
}