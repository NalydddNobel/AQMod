using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Weapons.Magic.Furystar;

public class FurystarBulletProj : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.Starfury);

    public override void SetDefaults() {
        Projectile.width = 42;
        Projectile.height = 28;
        Projectile.aiStyle = ProjAIStyleID.FallingStar;
        Projectile.friendly = true;
        Projectile.penetrate = 2;
        Projectile.scale = 0.8f;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.localNPCHitCooldown = 60;
        Projectile.usesLocalNPCImmunity = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White with { A = 0 } * Projectile.Opacity;
    }

    public override void AI() {
        Projectile.aiStyle = -1;

        Projectile.tileCollide = Projectile.Bottom.Y >= Projectile.ai[1];
        if (Projectile.soundDelay == 0) {
            Projectile.soundDelay = 20 + Main.rand.Next(40);
            SoundEngine.PlaySound(in SoundID.Item9, Projectile.position);
        }
        Projectile.alpha -= 15;
        int minimumAlpha = 150;
        if (Projectile.Center.Y >= Projectile.ai[1]) {
            minimumAlpha = 0;
        }
        if (Projectile.alpha < minimumAlpha) {
            Projectile.alpha = minimumAlpha;
        }
        Projectile.localAI[0] += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
        Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;

        if (Projectile.alpha < 200) {
            Vector2 screenSize = new(Main.screenWidth, Main.screenHeight);
            var hitbox = Projectile.Hitbox;
            if (hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + screenSize / 2f, screenSize + new Vector2(400f))) && Main.rand.NextBool(20)) {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0.2f, Main.rand.Next(16, 18));
            }
            if (Main.rand.NextBool(4)) {
                var d = Terraria.Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 127);
                d.velocity *= 0.7f;
                d.noGravity = true;
                d.velocity += Projectile.velocity * 0.3f;
                if (Main.rand.NextBool(2)) {
                    d.position -= Projectile.velocity * 4f;
                }
            }
        }

        Lighting.AddLight(Projectile.Center, 1f, 0.1f, 0.6f);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (target.immortal) {
            return;
        }

        var player = Main.player[Projectile.owner];
        var color = Color.Lerp(Color.Cyan, Color.Blue, Main.rand.NextFloat(0.15f, 0.85f));
        float scale = Main.rand.NextFloat(0.2f, 0.4f);
        var particle = ModContent.GetInstance<FurystarParticles>().New();
        particle.Location = Main.rand.NextVector2FromRectangle(target.getRect());
        particle.Velocity = Projectile.velocity * 0.05f;
        particle.Color = color;
        particle.Scale = scale;

        particle = ModContent.GetInstance<FurystarParticles>().New();
        particle.Location = Main.rand.NextVector2FromRectangle(Main.player[Projectile.owner].getRect());
        particle.Velocity = Main.player[Projectile.owner].velocity * 0.05f;
        particle.Color = color;
        particle.Scale = scale;

        int healMana = 10;
        player.statMana = Math.Min(player.statMana + healMana, player.statManaMax2);
        CombatText.NewText(player.getRect(), CombatText.HealMana * 0.8f, healMana, dot: true);
    }

    public override void OnKill(int timeLeft) {
        SoundEngine.PlaySound(in SoundID.Item10, Projectile.Center);
        for (int i = 0; i < 10; i++) {
            Terraria.Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Pink, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, Alpha: 150, Scale: Main.rand.NextFloat(1f, 1.5f));
        }
        for (int i = 0; i < 3; i++) {
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18));
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        // what the fuck !?
        bool altColor = Main.tenthAnniversaryWorld;
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame = new(0, 0, texture.Width, texture.Height);
        Vector2 origin = frame.Size() / 2f;
        Texture2D trailTexture = TextureAssets.Extra[91].Value;
        Rectangle trailFrame = trailTexture.Frame();
        Vector2 trailOrigin = new(trailFrame.Width / 2f, 10f);

        Vector2 drawOffset = new(0f, Projectile.gfxOffY);
        Vector2 spinningPoint = Vector2.Zero;
        float timer = (float)Main.timeForVisualEffects / 60f;
        float scale = Projectile.scale + 0.1f;
        drawOffset += Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
        var fadedPink = altColor ? new Color(140, 30, 244) * 0.75f : new Color(194, 22, 134) * 0.75f;
        fadedPink.A = (byte)(fadedPink.A / 2);
        var whiteGold = Color.Lerp(Color.Gold, Color.White, 0.5f);
        whiteGold.A = (byte)(whiteGold.A / 4);
        whiteGold *= 0.85f;
        whiteGold *= 0.75f;
        var gold = Color.Gold with { A = 180 };

        var pinkColor = altColor ? new Color(22, 70, 244, 127) : new Color(194, 22, 134, 127);
        var purpleColor = altColor ? new Color(0, 0, 255) * 0.5f : new Color(180, 20, 255) * 0.75f * 0.3f;
        var whiteColor = new Color(255, 255, 255, 0) * 0.5f * 0.3f;
        float randomStupidNumber = Projectile.rotation * 0.5f % ((float)Math.PI * 2f);
        if (randomStupidNumber < 0f) {
            randomStupidNumber += (float)Math.PI * 2f;
        }
        randomStupidNumber /= (float)Math.PI * 2f;
        float starPulse = Utils.Remap(randomStupidNumber, 0.15f, 0.5f, 0f, 1f) * Utils.Remap(randomStupidNumber, 0.5f, 0.85f, 1f, 0f);
        starPulse = 1f - starPulse;
        var drawColor = Color.Lerp(gold, pinkColor, starPulse);
        var bigTrailColor = Color.Lerp(fadedPink, purpleColor, starPulse);
        var smallTrailColor = Color.Lerp(whiteGold, whiteColor, starPulse);
        scale += starPulse * 0.2f;

        var drawCoordinates = Projectile.Center - Main.screenPosition + drawOffset;
        float trailRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Main.EntitySpriteDraw(trailTexture, drawCoordinates, trailFrame, bigTrailColor, trailRotation, trailOrigin, 0.9f, SpriteEffects.None);
        var starDrawCoordinates = drawCoordinates - Projectile.velocity * 0.4f;
        for (float num207 = 0f; num207 < 1f; num207 += 0.5f) {
            float trailTimer = timer % 0.5f / 0.5f;
            trailTimer = (trailTimer + num207) % 1f;
            float colorMultiplier = trailTimer * 2f;
            if (colorMultiplier > 1f) {
                colorMultiplier = 2f - colorMultiplier;
            }
            Main.EntitySpriteDraw(trailTexture, starDrawCoordinates, trailFrame, smallTrailColor * colorMultiplier, trailRotation, trailOrigin, (0.5f + trailTimer * 0.5f) * 0.75f, 0);
        }
        Main.EntitySpriteDraw(texture, starDrawCoordinates, frame, drawColor, Projectile.rotation, origin, scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        return false;
    }
}