using AQMod.Effects.Trails;
using AQMod.Effects.Trails.Rendering;
using System.Collections.Generic;

namespace AQMod
{
    public static class Trail
    {
        public static readonly TrailLayer<TrailType> PreDrawProjectiles = new TrailLayer<TrailType>();

        internal static void UpdateTrails<T>(List<T> trails) where T : TrailType
        {
            for (int i = 0; i < trails.Count; i++)
            {
                if (!trails[i].Update())
                {
                    trails.RemoveAt(i);
                    i--;
                }
            }
        }

        internal static void RenderTrails<T>(List<T> trails) where T : TrailType
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].Render();
            }
        }
    }
}
