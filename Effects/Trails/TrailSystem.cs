using System.Collections.Generic;

namespace AQMod.Effects.Trails
{
    public class TrailSystem
    {
        public readonly TrailLayer<Trail> PreDrawProjectiles;

        public TrailSystem()
        {
            PreDrawProjectiles = new TrailLayer<Trail>();
        }

        internal void UpdateTrails<T>(List<T> trails) where T : Trail
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

        internal void RenderTrails<T>(List<T> trails) where T : Trail
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].Render();
            }
        }
    }
}
