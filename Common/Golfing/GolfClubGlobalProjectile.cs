﻿using Terraria.Audio;
using Terraria.GameContent.Golf;

namespace Aequus.Common.Golfing;

public class GolfClubGlobalProjectile : GlobalProjectile {
    public override System.Boolean AppliesToEntity(Projectile entity, System.Boolean lateInstantiation) {
        return entity.aiStyle == ProjAIStyleID.GolfClub;
    }

    public override System.Boolean PreAI(Projectile projectile) {
        GolfingSystem.SetGolfBallStatus(true);
        var player = Main.player[projectile.owner];
        if (player.channel || projectile.ai[0] != 0f) {
            return true;
        }

        for (System.Int32 i = 0; i < Main.maxProjectiles; i++) {
            var golfBall = Main.projectile[i];
            var shotVector = Main.MouseWorld - golfBall.Center;
            if (!golfBall.active || golfBall.ModProjectile is not IGolfBallProjectile golfBallProjectile || golfBall.owner != projectile.owner || !GolfHelper.ValidateShot(golfBall, player, ref shotVector) || !golfBallProjectile.PreHit(shotVector)) {
                continue;
            }

            System.Single dir = Main.rand.NextFloatDirection();
            for (System.Single progress = 0f; progress < 1f; progress += 0.1f) {
                var dust = Dust.NewDustPerfect(golfBall.Center, golfBallProjectile.GolfBallHitDustId, (MathHelper.TwoPi * progress + dir).ToRotationVector2() * 0.8f, 127);
                dust.fadeIn = 0f;
                if (progress % 0.2f == 0f) {
                    dust.velocity *= 0.4f;
                }
            }

            SoundEngine.PlaySound(SoundID.Item126, golfBall.Center);
            if (projectile.owner == Main.myPlayer) {
                var shotStrength = GolfHelper.CalculateShotStrength(projectile, golfBall);
                var velocity = Vector2.Normalize(shotVector) * shotStrength.AbsoluteStrength;
                GolfHelper.HitGolfBall(golfBall, velocity, shotStrength.RoughLandResistance);
                golfBallProjectile.OnHit(velocity, shotStrength);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, i);
            }
        }
        return true;
    }

    public override void PostAI(Projectile projectile) {
        GolfingSystem.SetGolfBallStatus(false);
    }
}