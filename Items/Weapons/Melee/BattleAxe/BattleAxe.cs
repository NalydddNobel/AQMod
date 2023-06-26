using Aequus.Buffs.Debuffs;
using Aequus.Common.GlobalTiles;
using Aequus.Common.Net.Sounds;
using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.BattleAxe {
    public class BattleAxe : ModItem {
        public override void SetDefaults() {
            Item.DefaultToAequusSword<BattleAxeProj>(34);
            Item.useTime /= 2;
            Item.SetWeaponValues(14, 5f, 16);
            Item.width = 30;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.axe = 10;
            Item.tileBoost = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.reuseDelay = 4;
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return true;
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override bool CanShoot(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }
    }
}