﻿using Aequus.Common.Projectiles.SentryChip;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles {
    public abstract class PlayerAttachedProjBase : ModProjectile {
        public int AttachedProj { get => (int)Projectile.ai[0] - 1; set => Projectile.ai[0] = value + 1; }

        public override void AI() {
        }

        /// <summary>
        /// Gets an owner. Returns a santank dummy player if the projOwner is greater than -1.
        /// </summary>
        /// <param name="player">The owner of this projectile.</param>
        /// <param name="aequus">The "AequusPlayer" instance on the player.</param>
        /// <param name="projOwner">The "Projectile owner". This defaults to -1 unless this projectile is spawned with a projectile owner, then it returns their index in <see cref="Main.projectile"/></param>
        /// <param name="santank">The Projectile owner's <see cref="SentryAccessoriesGlobalProj"/> instance. Defaults to null unless <paramref name="projOwner"/> is greater than -1.</param>
        public void GetOwnerValues(out Player player, out AequusPlayer aequus, out int projOwner, out SentryAccessoriesGlobalProj santank) {
            projOwner = AttachedProj;
            if (projOwner > -1) {
                projOwner = Helper.FindProjectileIdentity(Projectile.owner, projOwner);
                if (projOwner == -1 || !Main.projectile[projOwner].active || !Main.projectile[projOwner].TryGetGlobalProjectile<SentryAccessoriesGlobalProj>(out var value)) {
                    Projectile.Kill();
                    player = null;
                    aequus = null;
                    santank = null;
                    return;
                }

                player = value.dummyPlayer;
                aequus = value.dummyPlayer?.GetModPlayer<AequusPlayer>();
                santank = value;
                Projectile.Center = Main.projectile[projOwner].Center;
                return;
            }

            player = Main.player[Projectile.owner];
            aequus = Main.player[Projectile.owner].GetModPlayer<AequusPlayer>();
            santank = null;
            Projectile.Center = Main.player[Projectile.owner].Center;
        }
    }
}