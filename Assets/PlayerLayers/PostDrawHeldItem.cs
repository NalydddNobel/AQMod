using AQMod.Assets.ItemOverlays;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Assets.PlayerLayers
{
    public class PostDrawHeldItem : PlayerLayerWrapper
    {
        public void Draw(PlayerDrawInfo info)
        {
            Player player = info.drawPlayer;
            Item item = player.inventory[player.selectedItem];
            if (info.shadow != 0f || player.frozen || (player.itemAnimation <= 0 || item.useStyle == 0) && (item.holdStyle <= 0 || player.pulley) || item.type <= ItemID.None || player.dead || item.noUseGraphic || item.noWet && player.wet || item.type < Main.maxItemTypes)
                return;
            ItemOverlayLoader.GetOverlay(item.type)?.DrawHeld(player, player.GetModPlayer<AQPlayer>(), item, info);
        }

        public override Action<PlayerDrawInfo> Apply()
        {
            return Draw;
        }
    }
}