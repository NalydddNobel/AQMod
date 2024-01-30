﻿using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.Enemies.DemonSiege.Keeper;

public class ChainedSoulProj : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileSets.PushableByTypeId.Add((ProjectileEntry)Type);
        ProjectileSets.DealsHeatDamage.Add((ProjectileEntry)Type);
    }

    public override void SetDefaults() {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 300;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(180, 180, 180, 50);
    }

    public override void AI() {
        if (Projectile.localAI[0] == 0f) {
            Projectile.localAI[0] = 1f;
            SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
            SoundEngine.PlaySound(AequusSounds.ChainedSoulAttack with { PitchVariance = 0.2f }, Projectile.Center);
        }

        Int32 d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
        Main.dust[d].velocity = Vector2.Lerp(Projectile.velocity, Main.dust[d].velocity, 0.5f);
        Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
        Main.dust[d].noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        if (Projectile.ai[0] > 0f) {
            Projectile.ai[0]--;
            return;
        }
        if (Projectile.lavaWet) {
            var player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            if (Projectile.Center.Y < player.Center.Y) {
                Projectile.velocity.Y += 1f;
                if (Projectile.velocity.Y > 40f) {
                    Projectile.velocity.Y = 40f;
                }
            }
            else {
                Projectile.velocity.Y -= 1f;
                if (Projectile.velocity.Y < -40f) {
                    Projectile.velocity.Y = -40f;
                }
            }
        }
        else {
            Projectile.velocity.Y += 1f;
            if (Projectile.velocity.Y > 20f) {
                Projectile.velocity.Y = 20f;
            }
        }
        if (Math.Abs(Projectile.velocity.X) < 1f) {
            Projectile.velocity.X *= 0.98f;
            if (Projectile.timeLeft > 60) {
                Projectile.timeLeft = 60;
            }
        }
    }

    public override Boolean TileCollideStyle(ref Int32 width, ref Int32 height, ref Boolean fallThrough, ref Vector2 hitboxCenterFrac) {
        fallThrough = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)].position.Y
            > Projectile.position.Y + Projectile.height;
        return true;
    }

    public override void OnKill(Int32 timeLeft) {
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            Int32 p = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChainedSoulExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Vector2 position = Projectile.Center - new Vector2(Main.projectile[p].width / 2f, Main.projectile[p].height / 2f);
            Main.projectile[p].position = position;
        }

        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        SoundEngine.PlaySound(AequusSounds.ChainedSoulAttackExplode, Projectile.position);

        var bvelo = -Projectile.velocity * 0.4f;
        for (Int32 i = 0; i < 3; i++) {
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, bvelo * 0.2f, 61 + Main.rand.Next(3));
        }
        for (Int32 i = 0; i < 12; i++) {
            Int32 d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
            Main.dust[d].noGravity = true;
        }
        for (Int32 i = 0; i < 30; i++) {
            Int32 d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
            Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
            Main.dust[d].noGravity = true;
        }
    }
}
