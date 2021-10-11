using AQMod.Assets;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerLayers
{
    public class PostDrawBody : PlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            AQMod.ArmorOverlays.InvokeArmorOverlay(ArmorOverlayType.Body, info);
        }
    }
}