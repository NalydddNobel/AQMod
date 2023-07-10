using Aequus.Common.EntitySources;
using Aequus.Common.Items.SentryChip;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.SentryChip {
    public class BoneGloveInteraction : SentryInteraction {
        public override void Load(Mod mod) {
            AddTo(ItemID.BoneGlove);
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            info.DummyPlayer.boneGloveTimer--;
        }

        public override void OnSentryCreateProjectile(IEntitySource source, Projectile newProjectile, AequusProjectile newAequusProjectile, SentryAccessoryInfo info) {
            if (source is IEntitySource_WithStatsFromItem || Main.myPlayer != info.Owner || newProjectile.type == ProjectileID.BoneGloveProj || info.DummyPlayer.boneGloveTimer > 0) {
                return;
            }

            info.DummyPlayer.boneGloveTimer = 60;
            var center = newProjectile.Center;
            var velocity = newProjectile.DirectionTo(info.DummyPlayer.ApplyRangeCompensation(0.2f, center, center + Vector2.Normalize(newProjectile.velocity) * 100f)) * 10f;
            Projectile.NewProjectile(newProjectile.GetSource_ItemUse(info.Accessory, "Aequus:CrownOfBlood"), center, velocity, ProjectileID.BoneGloveProj, 25, 5f, info.Owner);
        }
    }
}