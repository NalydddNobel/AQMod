using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class HyperCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accHyperCrystal = Item;
            if (aequus.hyperCrystalCooldownMax > 0)
            {
                aequus.hyperCrystalCooldownMax = Math.Max(aequus.hyperCrystalCooldownMax / 2, 1);
            }
            else
            {
                aequus.hyperCrystalCooldownMax = 50;
            }
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cHyperCrystal = dyeItem.dye;
        }
    }
}