using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Ultranium : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(gold: 1);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().accFocusCrystalRadius = 480f;
            player.GetModPlayer<AQPlayer>().accFocusCrystalDamage = 0.25f;
            player.GetModPlayer<AQPlayer>().accFocusCrystalVisible = !hideVisual;
            player.GetModPlayer<AQPlayer>().cosmicRadiationFlask = true;
        }
    }
}