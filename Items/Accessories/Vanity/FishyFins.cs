using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity
{
    public class FishyFins : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 10);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.vanity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Item.color = Main.LocalPlayer.skinColor;
            return null;
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedEars = Type;
                player.Aequus().cEars = dyeItem.dye;
            }
        }
    }
}