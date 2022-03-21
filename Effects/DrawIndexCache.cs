using System.Collections.Generic;

namespace AQMod.Effects
{
    public sealed class DrawIndexCache
    {
        public readonly List<int> Indices;
        public int Count => Indices.Count;

        public bool drawingNow;

        public DrawIndexCache()
        {
            Indices = new List<int>();
        }

        public void Clear()
        {
            Indices.Clear();
            drawingNow = false;
        }

        public void Add(int item)
        {
            Indices.Add(item);
        }
    }
}