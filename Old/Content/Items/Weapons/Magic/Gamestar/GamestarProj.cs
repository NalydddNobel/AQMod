using Aequus.Core.Particles;
using Aequus.DataSets;
using System;

namespace Aequus.Old.Content.Items.Weapons.Magic.Gamestar;

public class GamestarProj : ModProjectile {
    public override string Texture => AequusTextures.None.Path;

    public override void SetStaticDefaults() {
        this.SetTrail(10);
        ProjectileDataSet.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 180;
        Projectile.alpha = 200;
        Projectile.penetrate = 10;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI() {
        Projectile.UpdateShimmerReflection();
        if ((int)Projectile.ai[0] > 0f) {
            int target = (int)Projectile.ai[0] - 1;
            if ((int)Projectile.ai[1] <= 0) {
                Projectile.ai[1] = Main.rand.Next(1, 5) * 10;
                if (Main.myPlayer == Projectile.owner) {
                    Projectile.velocity = Vector2.Normalize(Main.npc[target].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                    Projectile.netUpdate = true;
                }
            }
            Projectile.ai[1]--;
        }
        Projectile.velocity *= 0.9975f;
        if (Main.netMode == NetmodeID.Server || !Cull2D.Rectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(100f)))) {
            return;
        }

        Particle<GamestarBits.Particle>.New().Setup(Projectile.Center, 20);

        for (int i = 0; i < Projectile.oldPos.Length; i++) {
            if (Projectile.oldPos[i] == Vector2.Zero || Main.rand.NextFloat(1f) < 0.33f) {
                continue;
            }

            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight,
                Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 40, Scale: Main.rand.NextFloat(0.8f, 1.5f));
            d.noGravity = true;

            int size = Main.rand.Next(8, 14);
            Vector2 spawnCoords = Projectile.oldPos[i] + Projectile.Size / 2f + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
            Particle<GamestarBits.Particle>.New().Setup(spawnCoords, size);
        }
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        fallThrough = true;
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        Projectile.penetrate--;
        if (Projectile.penetrate == 0) {
            return true;
        }
        if (Projectile.velocity.X != oldVelocity.X) {
            Projectile.velocity.X = -oldVelocity.X;
        }
        if (Projectile.velocity.Y != oldVelocity.Y) {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        return false;
    }

    private void SpawnParticles(Entity target) {
        if (!Cull2D.Rectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(100f)), 200, 200)) {
            return;
        }

        int amt = Math.Max((target.width + target.height) / 30, 5);
        foreach (var t in Particle<GamestarBits.Particle>.NewMultipleReduced(amt, 5)) {
            Vector2 spawnCoords = target.Center + new Vector2(Main.rand.NextFloat(-target.width, target.width), Main.rand.NextFloat(-target.height, target.height));
            t.Setup(spawnCoords, 20);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        target.AddBuff(ModContent.BuffType<GamestarDebuff>(), 480);
        if (Projectile.ai[0] + 1f <= target.whoAmI) {
            Projectile.ai[0] = target.whoAmI + 1;
            Projectile.timeLeft += 30;
        }
        Projectile.damage = (int)(Projectile.damage * 0.8f);
        if (Main.netMode != NetmodeID.Server) {
            SpawnParticles(target);
        }
        for (int i = (int)Projectile.ai[0]; i < Main.maxNPCs; i++) {
            if (Main.npc[i].CanBeChasedBy(Projectile)) {
                Projectile.velocity = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                return;
            }
        }
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].CanBeChasedBy(Projectile)) {
                Projectile.velocity = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                Projectile.ai[0] = 0f;
                break;
            }
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        if (Main.netMode != NetmodeID.Server) {
            SpawnParticles(target);
        }
    }
}
