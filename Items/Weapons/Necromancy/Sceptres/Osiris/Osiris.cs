﻿using Aequus.Common;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Osiris {
    [AutoloadGlowMask]
    [WorkInProgress]
    public class Osiris : SceptreBase {
        public override Color GlowColor => Color.Yellow;
        public override int DustSpawn => DustID.YellowStarDust;

        public override void SetDefaults() {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(75, 1f, 0);
            Item.shootSpeed = 12.5f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 4);
            Item.mana = 15;
            Item.UseSound = SoundID.Item8;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddIngredient(ItemID.DarkShard, 2)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.RainbowRod);
#endif
        }
    }
}