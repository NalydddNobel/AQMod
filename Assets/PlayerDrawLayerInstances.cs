using AQMod.Common.PlayerLayers;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public class PlayerDrawLayerInstances
    {
        public static readonly PlayerLayer postDrawHead = new PostDrawHead().GetLayer();
        public static readonly PlayerLayer postDrawBody = new PostDrawBody().GetLayer();
        public static readonly PlayerLayer postDrawHeldItem = new PostDrawHeldItem().GetLayer();
        public static readonly PlayerLayer preDraw = new PreDraw().GetLayer();
        public static readonly PlayerLayer postDraw = new PostDraw().GetLayer();
        public static readonly PlayerLayer postDrawWings = new PostDrawWings().GetLayer();

        public static readonly PlayerHeadLayer postDrawHeadHead = new PostDrawHeadHead().GetLayer();
    }
}