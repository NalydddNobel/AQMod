using AQMod.Items.DrawOverlays;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.PlayerLayers
{
    public sealed class PostDrawHeldItem : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HeldItem);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            var item = player.inventory[player.selectedItem];

            if (drawInfo.shadow != 0f || player.frozen || ((player.itemAnimation <= 0 || item.useStyle == 0) &&
                (item.holdStyle <= 0 || player.pulley)) || item.type <= ItemID.None || player.dead || item.noUseGraphic ||
                (item.noWet && player.wet) || item.type < Main.maxItemTypes)
            {
                return;
            }

            if (item.ModItem is IItemOverlaysPlayerDraw itemOverlay)
            {
                itemOverlay.PlayerDraw.DrawUse(player, player.GetModPlayer<AQPlayer>(), item, drawInfo);
            }
        }
    }
}