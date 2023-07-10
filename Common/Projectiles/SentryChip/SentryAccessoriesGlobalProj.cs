using Aequus.Common.Items.SentryChip;
using Aequus.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles.SentryChip {
    [LegacyName("SentryAccessoriesManager")]
    public class SentryAccessoriesGlobalProj : GlobalProjectile {
        public Player dummyPlayer;
        public bool appliedItemStatChanges;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) {
            return projectile.sentry;
        }

        public override GlobalProjectile Clone(Projectile projectile, Projectile projectileClone) {
            var clone = (SentryAccessoriesGlobalProj)base.Clone(projectile, projectileClone);
            if (dummyPlayer != null)
                clone.dummyPlayer = AequusPlayer.ProjectileClone(dummyPlayer);
            return clone;
        }

        public override void SetDefaults(Projectile projectile) {
            dummyPlayer = null;
            appliedItemStatChanges = false;
        }

        public override void PostAI(Projectile projectile) {
            if (projectile.hostile || projectile.owner < 0 || projectile.owner >= Main.maxPlayers || Main.player[projectile.owner].Aequus().accSentryInheritence == null) {
                dummyPlayer = null;
            }
        }

        private void InnerUpdateInheritence(Projectile projectile) {
            dummyPlayer ??= AequusPlayer.ProjectileClone(Main.player[projectile.owner]);
            dummyPlayer.active = true;
            dummyPlayer.dead = false;
            dummyPlayer.Center = projectile.Center;
            dummyPlayer.velocity = projectile.velocity;
            PlayerLoader.PreUpdate(dummyPlayer);
            dummyPlayer.ResetEffects();
            dummyPlayer.UpdateDyes();
            dummyPlayer.whoAmI = projectile.owner;
            dummyPlayer.Aequus().projectileIdentity = projectile.identity;
            dummyPlayer.wetCount = projectile.wetCount;
            dummyPlayer.wet = Collision.WetCollision(projectile.position, projectile.width, projectile.height);
            dummyPlayer.lavaWet = projectile.lavaWet;
            dummyPlayer.honeyWet = projectile.honeyWet;

            SentryAccessoriesDatabase.ApplyOnAIInteractions(projectile, this);
            appliedItemStatChanges = true;

            for (int i = 0; i < ProjectileLoader.ProjectileCount; i++) {
                dummyPlayer.ownedProjectileCounts[i] = 0;
            }
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].TryGetAequus(out var ownedAequusProjectile) && ownedAequusProjectile.sourceProjIdentity == projectile.identity) {
                    dummyPlayer.ownedProjectileCounts[Main.projectile[i].type]++;
                }
            }
            PlayerLoader.PostUpdate(dummyPlayer);
            dummyPlayer.numMinions = 0;
            dummyPlayer.slotsMinions = 0f;
        }

        public void UpdateInheritance(Projectile projectile) {
            if (projectile.hostile || !projectile.sentry || projectile.TurretShouldPersist() || projectile.owner < 0 || projectile.owner >= Main.maxPlayers) {
                appliedItemStatChanges = false;
                return;
            }

            AequusProjectile.pWhoAmI = projectile.whoAmI;
            AequusProjectile.pIdentity = projectile.identity;

            try {
                InnerUpdateInheritence(projectile);
            }
            catch {

            }

            AequusProjectile.pIdentity = -1;
            AequusProjectile.pWhoAmI = -1;
        }
    }
}