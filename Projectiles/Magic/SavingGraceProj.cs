using Aequus.Common.DataSets;
using Aequus.Content;
using Aequus.Particles.Dusts;
using System;
using Terraria.Audio;

namespace Aequus.Projectiles.Magic;
public class SavingGraceProj : ModProjectile {
    public float auraScale;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 1;
        LegacyPushableEntities.AddProj(Type);
        ProjectileSets.DealsHeatDamage.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 60;
        Projectile.height = 60;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 360;
        Projectile.alpha = 200;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.scale = 0.1f;
        Projectile.localNPCHitCooldown = 500;
        Projectile.extraUpdates = 2;
    }

    public override void AI() {
        Projectile.ShimmerReflection();
        if (Projectile.alpha < 180 && Projectile.scale < 0.4f) {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MonoDust>(), Vector2.Zero, newColor: new Color(120, 255, 150, 0), Scale: 2f);
        }
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 7;
            if (Projectile.alpha <= 0) {
                Projectile.alpha = 0;
            }
        }
        if (Projectile.localAI[0] == 0) {
            var v = Vector2.Normalize(Projectile.velocity);
            Projectile.localAI[0] = v.X;
            Projectile.localAI[1] = v.Y;
            Projectile.scale /= (1 + Projectile.scale);
        }
        if ((int)Projectile.ai[1] == 0) {
            for (int i = -1; i <= 1; i += 2) {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(i * 0.3f) * 0.8f, Type, Projectile.damage / 2, Projectile.knockBack / 2f, Projectile.owner, Projectile.ai[0], 1f);
            }
            Projectile.ai[1]++;
        }
        Projectile.ai[0]++;
        int timer = (int)Projectile.ai[0];
        int timeBetweenTurns = 6;
        Projectile.velocity *= 0.94f;
        if (Projectile.velocity.Length() > 1f) {
            if (timer % timeBetweenTurns == 0) {
                int dir = (timer % (timeBetweenTurns * 2) == 0 ? 1 : -1);
                float rotationAmount = Main.rand.NextFloat(0.2f, 0.6f) * dir;
                Projectile.velocity = (new Vector2(Projectile.localAI[0], Projectile.localAI[1]) * Projectile.velocity.Length()).RotatedBy(rotationAmount);
            }
            Projectile.Resize((int)(180 * Projectile.scale), (int)(180 * Projectile.scale));
            Projectile.scale += 0.005f;
            Projectile.scale *= 1.01f;
        }
        else {
            if (auraScale < 1f) {
                auraScale += 0.005f;
                auraScale *= 1.02f;
                if (auraScale > 1f) {
                    auraScale = 1f;
                }
            }
            Projectile.velocity *= 0.9f;
        }
        if (CanHeal()) {
            int team = Main.player[Projectile.owner].team;
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && Main.player[i].team == team && Projectile.Distance(Main.player[i].Center) < 80f) {
                    HealPlayer(i);
                    break;
                }
            }
        }
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        fallThrough = true;
        return true;
    }

    public override void OnKill(int timeLeft) {
        for (int i = 0; i < 20; i++) {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(70, 200, 100, 0), Scale: 2f);
        }
    }

    public void HealPlayer(int plr) {
        SoundEngine.PlaySound(AequusSounds.savingGraceHeal, Main.player[plr].Center);
        DoHealLine(Projectile.Center, Main.player[plr].Center);
        if (plr != Projectile.owner)
            DoHealLine(Projectile.Center, Main.player[Projectile.owner].Center);

        if (Main.myPlayer == plr) {
            Main.player[plr].Heal(Projectile.damage / 10);
        }
        if (Main.myPlayer == Projectile.owner && plr != Projectile.owner) {
            int healMana = 20;
            Main.player[plr].ManaEffect(healMana);
            Main.player[plr].statMana = Math.Min(Main.player[plr].statMana + healMana, Main.player[plr].statManaMax2);
        }
        Projectile.Kill();
    }

    public void DoHealLine(Vector2 start, Vector2 end) {
        var diff = end - start;
        var v = Vector2.Normalize(diff);
        int distance = Math.Min((int)diff.Length(), 1000);
        float off = 0f;
        float offDir = 0f;
        var offVector = v.RotatedBy(MathHelper.PiOver2);
        var color = new Color(70, 200, 100, 0);
        for (int i = 8; i < distance; i++) {
            if (i < 20) {
                offDir /= 2f;
            }
            else if (i % 16 == 0) {
                offDir += Main.rand.NextFloat(-1f, 1f);
                offDir /= 2f;
            }
            off += offDir;
            Dust.NewDustPerfect(start + v * i + offVector * off, ModContent.DustType<MonoDust>(), Vector2.Zero, newColor: color, Scale: 1f);
        }
    }

    public bool CanHeal() {
        return Projectile.velocity.Length() < 1f && auraScale > 0.8f && Projectile.damage > 10;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.damage = (int)(Projectile.damage * 0.75f);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Color.White.UseA(200) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

        var aura = ModContent.Request<Texture2D>($"{Texture}_Aura").Value;
        var auraOrigin = aura.Size() / 2f;
        var auraColor = Color.Lime.UseA(0) * 0.1f * Projectile.Opacity;
        Main.spriteBatch.Draw(aura, Projectile.position + offset - Main.screenPosition, null, auraColor, Projectile.rotation, auraOrigin, auraScale * 0.5f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(aura, Projectile.position + offset - Main.screenPosition, null, auraColor, Projectile.rotation + MathHelper.PiOver2, auraOrigin, auraScale * 0.5f, SpriteEffects.None, 0f);

        return false;
    }
}