using Aequus.Buffs.Debuffs;
using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Items.Weapons.Melee.Demon.Cauterizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Demon {
    [AutoloadGlowMask]
    public class DemonSword : ModItem {
        public override bool IsLoadingEnabled(Mod mod) {
            return Aequus.DevelopmentFeatures;
        }

        public override void SetStaticDefaults() {
            DemonSiegeSystem.RegisterSacrifice(new(ModContent.ItemType<HellsBoon.HellsBoon>(), Type, UpgradeProgressionType.Hardmode));
            DemonSiegeSystem.RegisterSacrifice(new(ModContent.ItemType<Cauterizer.Cauterizer>(), Type, UpgradeProgressionType.Hardmode));
        }

        public override void SetDefaults() {
            Item.DefaultToAequusSword<DemonSwordProj>(38);
            Item.SetWeaponValues(49, 4.5f, 6);
            Item.width = 24;
            Item.height = 24;
            Item.scale = 1f;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityDemonSiegeTier2;
            Item.value = ItemDefaults.ValueDemonSiegeTier2;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(100);
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuffs(240, 1, CorruptionHellfire.Debuffs);
            target.AddBuffs(240, 1, CrimsonHellfire.Debuffs);
        }
    }
}
