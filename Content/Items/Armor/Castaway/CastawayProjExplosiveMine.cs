using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequu2.Content.Items.Armor.Castaway;

public class CastawayProjExplosiveMine : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.SpikyBall);

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.timeLeft = 360;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 16;
        Projectile.friendly = true;
    }

    public override void AI() {
        if (Projectile.ai[0] == 0f) {
            Projectile.ai[0] = 1f;
            Projectile.timeLeft += Main.rand.Next(40);
        }
        if (Projectile.velocity.Y == 0f) {
            Projectile.velocity.X *= 0.9f;
        }

        int amount = 0;
        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && Main.projectile[i].type == Type && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].timeLeft >= Projectile.timeLeft) {
                amount++;
            }
        }
        if (amount >= CastawayArmor.MaxBallsOut) {
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 16);
        }

        if (Projectile.timeLeft == 12) {
            for (int i = 0; i < 30; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Alpha: 100, Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                d.velocity *= 2f;
            }
            for (int i = 0; i < 10; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                d.velocity *= 3f;
                d.fadeIn = d.scale + 0.2f;
            }
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.damage *= 3;
            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;
            Projectile.Resize(Projectile.width * 8, Projectile.height * 8);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        if (Projectile.tileCollide) {
            Projectile.velocity.Y += 0.3f;
        }
        else {
            Projectile.velocity *= 0.8f;
        }

        if (Projectile.velocity.Length() > 2f && Main.rand.NextBool(10)) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, Alpha: 100, Scale: Main.rand.NextFloat(0.8f, 1f));
            d.noGravity = true;
            d.fadeIn = d.scale + 0.2f;
        }
        Projectile.rotation += Projectile.velocity.X * 0.33f;

        Projectile.localAI[0] += Math.Clamp(1f - Projectile.timeLeft / 240f, 0.1f, 1f);
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        return false;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var drawCoordinates = Projectile.Center - Main.screenPosition + new Vector2(0f, 4f);
        var flareColor = new Color(255, 150, 40, 0);
        if (Projectile.timeLeft < 16) {
            float animation = 1f - MathF.Pow(Projectile.timeLeft / 16f, 3f);
            var flareTexture = AequusTextures.Flare2;
            float flareScale = Projectile.scale * MathF.Sin(animation * MathHelper.Pi) * 0.75f;
            flareColor *= animation;
            var flareOrigin = flareTexture.Size() / 2f;
            var explosionFrame = AequusTextures.GenericExplosion.Frame(verticalFrames: 7, frameY: 7 - Projectile.timeLeft / 3);
            Main.EntitySpriteDraw(AequusTextures.GenericExplosion, drawCoordinates, explosionFrame, flareColor, Projectile.rotation, explosionFrame.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(AequusTextures.BloomStrong, drawCoordinates, null, flareColor * 0.33f * flareScale, Projectile.rotation, AequusTextures.BloomStrong.Size() / 2f, 1f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(flareTexture, drawCoordinates, null, flareColor, Projectile.rotation, flareOrigin, flareScale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(flareTexture, drawCoordinates, null, flareColor, Projectile.rotation + MathHelper.PiOver2, flareOrigin, flareScale, SpriteEffects.None, 0f);
        }
        else {
            float wave = Helper.Oscillate(Projectile.localAI[0], 0f, 1f);
            Main.EntitySpriteDraw(texture, drawCoordinates, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(texture, drawCoordinates, null, flareColor * wave, Projectile.rotation, texture.Size() / 2f, Projectile.scale + 0.1f * wave, SpriteEffects.None, 0f);
        }
        return false;
    }
}