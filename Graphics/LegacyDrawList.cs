using System.Collections.Generic;

namespace Aequus.Graphics
{
    public class LegacyDrawList
    {
        public int Count => List.Count;

        internal readonly List<int> List;
        public bool renderingNow;
        public bool RenderingNow => renderingNow || ForceRender;
        public static bool ForceRender;

        public LegacyDrawList()
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
    public class DrawList<T> : List<T>
    {
        public bool renderNow;
        public bool Render => renderNow || EffectsSystem.LegacyForceRenderDrawlists;
    }
}