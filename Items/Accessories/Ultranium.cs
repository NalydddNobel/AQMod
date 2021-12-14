using AQMod.Items.DrawOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Ultranium : ModItem, IItemOverlaysWorldDraw
    {
        private static GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<Ultranium>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeCrit += 5;
            player.GetModPlayer<AQPlayer>().hyperCrystal = true;
        }
    }
}