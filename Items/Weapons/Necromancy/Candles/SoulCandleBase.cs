﻿using Aequus.Buffs.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Candles {
    public abstract class SoulCandleBase : ModItem {
        public const int ItemHoldStyle = ItemHoldStyleID.HoldFront;

        public override void SetStaticDefaults() {
#if !DEBUG
            Item.ResearchUnlockCount = 0;
#endif
        }

        protected void DefaultToCandle(int summonDamage) {
            Item.holdStyle = ItemHoldStyle;
            Item.DamageType = Aequus.NecromancyClass;
            Item.damage = summonDamage;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.mana = 50;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.SummonWeapon;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            for (int j = 0; j < tooltips.Count; j++) {
                if (tooltips[j].Name == "Knockback") {
                    tooltips.RemoveAt(j);
                    j--;
                }
            }
            base.ModifyTooltips(tooltips);
        }

        public override bool CanUseItem(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0 && Collision.CanHitLine(player.position, player.width, player.height, Main.MouseWorld, 2, 2);
        }

        public override bool? UseItem(Player player) {
            player.AddBuff(ModContent.BuffType<RitualBuff>(), 300);
            return null;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
            velocity *= 0.1f;
        }

        public override bool MagicPrefix() {
            return true;
        }
    }
}