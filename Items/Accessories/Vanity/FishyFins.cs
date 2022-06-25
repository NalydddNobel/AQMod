﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity
{
    public class FishyFins : ModItem, Hooks.IUpdateItemDye
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 10);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.vanity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Item.color = Main.LocalPlayer.skinColor;
            return null;
        }

        void Hooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().ears = Type;
            player.Aequus().cEars = dyeItem.dye;
        }
    }
}