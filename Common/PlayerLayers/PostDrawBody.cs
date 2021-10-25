using AQMod.Assets;
using AQMod.Assets.PlayerLayers.EquipOverlays;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerLayers
{
    public class PostDrawBody : PlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            AQMod.ArmorOverlays.InvokeArmorOverlay(EquipLayering.Body, info);
        }
    }
}