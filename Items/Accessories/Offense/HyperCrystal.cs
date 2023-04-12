using Aequus.Common.Recipes;
using Aequus.Content;
using Aequus.Items.Materials;
using Aequus.Items.Weapons.Melee.Heavy;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense
{
    public class HyperCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;

            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
            Item.hasVanityEffects = true;
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
                aequus.hyperCrystalCooldownMax = 20;
            }
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cHyperCrystal = dyeItem.dye;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<StariteMaterial>(20)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}