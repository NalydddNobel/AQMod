using Aequus.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Accessories
{
    public class CrownOfTheGrounded : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = Item.buyPrice(gold: 7, silver: 50);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.EquipAccessory
                && !Main.LocalPlayer.Aequus().grounded)
            {
                return Color.Gray;
            }
            return null;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accGroundCrownCrit += 10;
        }
    }
}