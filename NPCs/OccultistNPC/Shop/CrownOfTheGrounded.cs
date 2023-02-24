using Aequus.Items;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.NPCs.OccultistNPC.Shop
{
    public class CrownOfTheGrounded : ModItem, ItemHooks.IUpdateItemDye
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
            Item.canBePlacedInVanityRegardlessOfConditions = true;
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

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().crown = Type;
                player.Aequus().cCrown = dyeItem.dye;
            }
        }
    }
}