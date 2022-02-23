using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class MagmalbulbiaStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            this.Glowmask();
        }

        private static Color getGlowmaskColor() => Color.Lerp(new Color(188, 175, 135, 0), new Color(144, 130, 111, 0), ((float)Math.Sin(Main.GlobalTime * 1.1f) + 1f) / 2f);

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.useTime = 26;
            item.useAnimation = 26;
            item.width = 50;
            item.height = 50;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.MagmalbulbiaStaff>();
            item.shootSpeed = 24.11f;
            item.mana = 11;
            item.autoReuse = true;
            item.UseSound = SoundID.Item95;
            item.value = AQItem.Prices.PostMechsEnergyWeaponValue;
            item.knockBack = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GebulbaStaff>());
            recipe.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 10);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}