using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerData.Layers
{
    public class PostDrawHeldItem : TempPlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            var player = info.drawPlayer;
            Item item = player.inventory[player.selectedItem];
            if (info.shadow != 0f || player.frozen || (player.itemAnimation <= 0 || item.useStyle == 0) && (item.holdStyle <= 0 || player.pulley) || item.type <= ItemID.None || player.dead || item.noUseGraphic || item.noWet && player.wet || item.type < Main.maxItemTypes)
                return;
            AQMod.ItemOverlays.GetOverlay(item.type)?.DrawHeld(player, player.GetModPlayer<AQPlayer>(), item, info);
        }
    }
}