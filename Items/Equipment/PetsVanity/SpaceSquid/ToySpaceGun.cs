﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.SpaceSquid {
    public class ToySpaceGun : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToVanitypet(ModContent.ProjectileType<SpaceSquidPet>(), ModContent.BuffType<SpaceSquidBuff>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.Aequus().itemGravityCheck = 255;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }
}