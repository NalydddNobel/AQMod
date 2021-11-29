using AQMod.Common.Graphics.PlayerEquips;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerData.Layers
{
    public class PostDrawBody : TempPlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            AQMod.ArmorOverlays.InvokeArmorOverlay(EquipLayering.Body, info);
        }
    }
}