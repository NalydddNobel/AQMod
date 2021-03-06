using System.Collections.Generic;

namespace Aequus.Graphics
{
    public sealed class DrawList
    {
        public int Count => List.Count;

        internal readonly List<int> List;
        public bool renderingNow;
        public bool RenderingNow => renderingNow || ForceRender;
        public static bool ForceRender;

        public DrawList()
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