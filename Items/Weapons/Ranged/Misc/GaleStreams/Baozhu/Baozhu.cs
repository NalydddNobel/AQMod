﻿using Aequus.Common.Items;
using Aequus.Items.Materials.GaleStreams;
using Aequus.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Misc.GaleStreams.Baozhu {
    [AutoloadGlowMask]
    public class Baozhu : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults() {
            Item.damage = 43;
            Item.crit = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.width = 16;
            Item.height = 16;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.ValueGaleStreams;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BaozhuProj>();
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 15)
                .AddIngredient<Fluorescence>(15)
                .AddTile(TileID.Anvils)
                .Register()
                .Clone().ReplaceItem(ItemID.CobaltBar, ItemID.PalladiumBar).Register();
        }
    }
}