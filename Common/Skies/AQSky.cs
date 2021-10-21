using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace AQMod.Common.Skies
{
    /// <summary>
    /// An abstract custom sky which contains many useful methods.
    /// </summary>
    public abstract class AQSky : CustomSky
    {
        protected const float RecommendedMinDepth = 0f;
        protected const float RecommendedMaxDepth = 10f;

        protected UnifiedRandom _random;

        public AQSky()
        {
            _random = new UnifiedRandom();
        }

        public bool SkyDepth(float maxDepth, float minDepth)
        {
            return maxDepth == float.MaxValue && minDepth != float.MaxValue;
        }

        protected interface IDepth
        {
            float Depth { get; }
        }

        protected TDepth[] SortDepth<TDepth>(TDepth[] array) where TDepth : IDepth
        {
            return SortDepth(new List<TDepth>(array)).ToArray();
        }

        protected List<TDepth> SortDepth<TDepth>(List<TDepth> list) where TDepth : IDepth
        {
            list.Sort((depth, depth2) => depth.Depth.CompareTo(depth2.Depth));
            return list;
        }

        protected void GetDepthDrawingIndices<TDepth>(out int minI, out int maxI, float minDepth, float maxDepth, TDepth[] array) where TDepth : IDepth
        {
            GetDepthDrawingIndices(out minI, out maxI, minDepth, maxDepth, new List<TDepth>(array));
        }

        protected void GetDepthDrawingIndices<TDepth>(out int minI, out int maxI, float minDepth, float maxDepth, List<TDepth> list) where TDepth : IDepth
        {
            minI = -1;
            maxI = 0;
            for (int j = 0; j < list.Count; j++)
            {
                float depth = list[j].Depth;
                if (minI == -1 && depth < maxDepth)
                {
                    minI = j;
                }
                if (depth <= minDepth)
                {
                    break;
                }
                maxI = j;
            }
            if (minI == -1)
            {
                return;
            }
        }

        protected bool CanEvenSeeTheSkyAtAll()
        {
            if (Main.screenPosition.Y > Main.worldSurface * 16.0 || Main.gameMenu)
            {
                return false;
            }
            return true;
        }
    }
}