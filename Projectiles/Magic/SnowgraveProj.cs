﻿using Aequus.Buffs.Debuffs;
using Aequus.Particles.Dusts;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Projectiles.Magic;
public class SnowgraveProj : ModProjectile {
    public override void SetDefaults() {
        Projectile.width = 180;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.coldDamage = true;
        Projectile.timeLeft = 80;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 4;
    }

    public override void AI() {
        if ((int)Projectile.localAI[0] == 0) {
            Projectile.localAI[0] = Projectile.width / 6;
        }
        if (Projectile.ai[0] > 0f) {
            Projectile.ai[0]--;
            if (Projectile.ai[0] < 0f) {
                Projectile.ai[0] = 0f;
            }
        }
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 100);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        if (target.position.X + target.width / 2f < Main.player[Projectile.owner].position.X + Main.player[Projectile.owner].width / 2f) {
            modifiers.HitDirectionOverride = -1;
        }
        else {
            modifiers.HitDirectionOverride = 1;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        target.AddBuff(ModContent.BuffType<SnowgraveDebuff>(), 240);
        Projectile.damage = (int)(Projectile.damage * 0.5f);
        var center = target.Center;
        float l = target.Size.Length() / 4f;
        float r = Main.rand.NextFloat(-(MathHelper.TwoPi / 12f), MathHelper.TwoPi / 12f);
        for (int i = 0; i < 12; i++) {
            var normal = Vector2.UnitX.RotatedBy(i * (MathHelper.TwoPi / 12f) + r);
            Dust.NewDustPerfect(center + normal * l, ModContent.DustType<MonoDust>(), normal * Main.rand.NextFloat(2.5f, 7.5f), 0, new Color(80, 120, 255, 128), Main.rand.NextFloat(0.9f, 1.3f));
        }
        if (hit.Crit) {
            r = Main.rand.NextFloat(-(MathHelper.TwoPi / 4f), MathHelper.TwoPi / 4f);
            for (int i = 0; i < 4; i++) {
                var normal = Vector2.UnitX.RotatedBy(i * (MathHelper.TwoPi / 4f) + r);
                Dust.NewDustPerfect(center + normal * l, ModContent.DustType<MonoSparkleDust>(), normal * Main.rand.NextFloat(7f, 10f), 0, new Color(120, 160, 255, 128), Main.rand.NextFloat(1.2f, 1.5f));
            }
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        var center = Projectile.Center - Main.screenPosition;
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = new Rectangle(0, 0, texture.Width, texture.Height);
        var origin = frame.Size() / 2f;
        var color = Projectile.GetAlpha(lightColor);
        Main.spriteBatch.Draw(texture, center, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        for (int i = 0; i < 3; i++) {
            Main.spriteBatch.Draw(texture, new Vector2(center.X + Projectile.localAI[0] * i, center.Y), frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(center.X - Projectile.localAI[0] * i, center.Y), frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        }
        return false;
    }
}

public class SnowgraveProjSpawner : ModProjectile {
    public override string Texture => Aequus.BlankTexture;

    private bool _playedSound;

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 120;
        Projectile.coldDamage = true;
        Projectile.hide = true;
        Projectile.tileCollide = false;
    }

    public override bool? CanDamage() {
        return false;
    }

    public override void AI() {
        if (Main.netMode != NetmodeID.Server && !_playedSound) {
            _playedSound = true;
            SoundEngine.PlaySound(AequusSounds.snowgrave with { Volume = 0.75f, }, Projectile.Center);
        }
        Projectile.velocity.Y = 8f;
        if (Main.myPlayer == Projectile.owner) {
            int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0f, -28f), ModContent.ProjectileType<SnowgraveProj>(), Projectile.damage / 30, Projectile.knockBack, Projectile.owner);
            Main.projectile[p].localAI[0] = Main.projectile[p].width / 6;
            Main.projectile[p].localAI[0] -= Helper.Wave(Projectile.timeLeft * 0.2f, 0f, 18f);
        }
    }
}