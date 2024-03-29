﻿using Aequus.Buffs.Minion;
using Aequus.Common.Items;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.Glimmer;
using Aequus.Items.Weapons.Summon.StariteMinion;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.ScribbleNotebook {
    [AutoloadGlowMask]
    public class ScribbleNotebook : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 20;
            Item.knockBack = 5f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = ItemDefaults.ValueOmegaStarite;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.UseSound = SoundID.Item44;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<ScribbleNotebookBuff>();
            Item.shoot = ModContent.ProjectileType<ScribbleNotebookMinion>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback);
            return false;
        }
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<StariteMaterial>(14)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.NightsEdge);
        }
    }
}