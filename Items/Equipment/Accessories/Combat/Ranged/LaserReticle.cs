using Aequus.Common.Items.EquipmentBooster;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using System;
using Terraria.Localization;

namespace Aequus.Items.Equipment.Accessories.Combat.Ranged {
    [AutoloadGlowMask()]
    [LegacyName("PrecisionGloves")]
    public class LaserReticle : ModItem {
        /// <summary>
        /// Default Value: 0.5
        /// </summary>
        public static float BulletSpreadMultiplier = 0.5f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.PercentDifference(BulletSpreadMultiplier));

        public override void SetStaticDefaults() {
            AnalysisSystem.IgnoreItem.Add(Type);
            EquipBoostDatabase.Instance.SetEntry(this, base.Tooltip.WithFormatArgs(TextHelper.Create.PercentDifference(MathF.Pow(BulletSpreadMultiplier, 2f))));
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().bulletSpread *= BulletSpreadMultiplier;
        }
    }
}

namespace Aequus.Projectiles {
    public partial class AequusProjectile {
        public void OnSpawn_CheckSpread(Player owner, AequusPlayer aequus, Projectile projectile) {
            if (aequus.bulletSpread < 1f && projectile.DamageType.CountsAsClass(DamageClass.Ranged)) {
                float angle = projectile.velocity.ToRotation().AngleLerp(
                    projectile.AngleTo(Main.MouseWorld),
                    1f - aequus.bulletSpread
                );
                projectile.velocity = angle.ToRotationVector2() * projectile.velocity.Length();
            }
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public float bulletSpread;
    }
}