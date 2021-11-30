using AQMod.Common.PlayerData.Layers;
using Terraria.ModLoader;

namespace AQMod.Assets.Graphics.LegacyPlayerLayers
{
    public static class PlayerLayersCache
    {
        public static readonly PlayerLayer preDraw = new PreDraw().GetLayer();
        public static readonly PlayerLayer postDraw = new PostDraw().GetLayer();

        public static readonly PlayerHeadLayer postDrawHeadHead = new HeadLayerPostDraw().GetLayer();
    }
}