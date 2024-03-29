﻿using Aequus.Buffs.Debuffs;
using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.Demon.Cauterizer {
    public class Cauterizer : ModItem {
        public override void SetStaticDefaults() {
            DemonSiegeSystem.RegisterSacrifice(new(ItemID.BloodButcherer, Type, EventTier.PreHardmode));
        }

        public override void SetDefaults() {
            Item.DefaultToAequusSword<CauterizerProj>(38);
            Item.SetWeaponValues(49, 4.5f, 6);
            Item.width = 24;
            Item.height = 24;
            Item.scale = 1f;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.autoReuse = true;
            Item.value = ItemDefaults.ValueDemonSiege;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(200);
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuffs(240, 1, CrimsonHellfire.Debuffs);
        }
    }
}