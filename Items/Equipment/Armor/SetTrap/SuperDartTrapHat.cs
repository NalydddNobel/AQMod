﻿using Aequus.Projectiles.Summon.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetTrap {
    [AutoloadEquip(EquipType.Head)]
    public class SuperDartTrapHat : DartTrapHat {
        public override int ProjectileShot => ModContent.ProjectileType<SuperDartTrapHatProj>();
        public override int TimeBetweenShots => base.TimeBetweenShots / 2;
        public override float Speed => base.Speed * 1.5f;
        public override int Damage => 200;

        public override void SetDefaults() {
            Item.width = 12;
            Item.height = 12;
            Item.defense = 10;
            Item.damage = Damage;
            Item.DamageType = DamageClass.Summon;
            Item.ArmorPenetration = 15;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player) {
            base.UpdateEquip(player);
            player.GetDamage(DamageClass.Summon) += 0.4f;
            player.maxMinions += 1;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<VenomDartTrapHat>()
                .AddIngredient(ItemID.SuperDartTrap)
                .AddIngredient(ItemID.ChlorophyteBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterBefore(ItemID.CopperBar);
        }
    }
}