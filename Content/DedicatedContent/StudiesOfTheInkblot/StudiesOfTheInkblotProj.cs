using System;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Content.DedicatedContent.StudiesOfTheInkblot;

public class StudiesOfTheInkblotProj : ModProjectile {
    public override void SetStaticDefaults() {
        Main.projFrames[Projectile.type] = 7;
    }

    public override void SetDefaults() {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.alpha = 275;
        Projectile.penetrate = -1;
    }

    public override Color? GetAlpha(Color lightColor) {
        return FrameToColor(Projectile.frame);
    }

    public override void AI() {
        if ((Int32)Projectile.ai[0] >= 100f) {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 120f && Projectile.localAI[1] <= 0f) {
                Projectile.position.X -= 40f;
                Projectile.position.Y -= 40f;
                Projectile.width += 80;
                Projectile.height += 80;
                Projectile.localAI[1] = 1f;
            }
            if (Projectile.ai[0] > 140f) {
                Projectile.ai[0] = 100f;
                Projectile.localAI[1] = 0f;
                Projectile.position.X += 40f;
                Projectile.position.Y += 40f;
                Projectile.width -= 80;
                Projectile.height -= 80;
            }
        }
        if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() < 1250f) {
            Projectile.timeLeft = 20;
        }
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 5;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }

        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * Projectile.ai[1], Projectile.velocity.Length() / Projectile.ai[1] * 0.1f);

        if (Projectile.alpha < 200) {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;
            Projectile.localAI[0] *= 0.8f;
            Single colorMultiplier = 1 - Projectile.alpha / 255f;
            Lighting.AddLight(Projectile.Center, Projectile.GetAlpha(default(Color)).ToVector3() * Projectile.scale * 0.5f * colorMultiplier);
        }
    }

    public override Boolean PreDraw(ref Color lightColor) {
        if (Projectile.alpha > 255) {
            return false;
        }
        if (Projectile.ai[1] == 0f) {
            Projectile.ai[1] = 20f;
        }
        Single colorMultiplier = 1 - Projectile.alpha / 255f;
        Single colorMultiplierSquared = colorMultiplier * colorMultiplier;
        var texture = TextureAssets.Projectile[Type].Value;
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        var spotlight = AequusTextures.Bloom;
        Main.spriteBatch.Draw(spotlight, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * colorMultiplierSquared, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.6f, SpriteEffects.None, 0f);
        var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, new Color(255, 255, 255, 255) * colorMultiplierSquared, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        if (Projectile.localAI[0] > Projectile.scale * 0.6f) {
            Main.spriteBatch.Draw(spotlight, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * colorMultiplier, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * Projectile.localAI[0], SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255) * colorMultiplier, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * Projectile.localAI[0] * 0.7f, SpriteEffects.None, 0f);
        }

        if (Projectile.ai[0] > 120f) {
            var explosionTexture = AequusTextures.GenericExplosion.Value;
            Int32 explosionFrameNumber = (Int32)((Projectile.ai[0] - 120f) / 4f);
            var explosionFrame = explosionTexture.Frame(verticalFrames: 7, frameY: explosionFrameNumber);
            var explosionOrigin = explosionFrame.Size() / 2f;
            Main.spriteBatch.Draw(explosionTexture, Projectile.position + offset - Main.screenPosition, explosionFrame, (Projectile.frame == 1 ? Color.Red : Color.Blue) with { A = 120 }, 0f, explosionOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
        }
        return false;
    }

    public static Color FrameToColor(Int32 frame) {
        switch (frame) {
            default:
                return new Color(255, 255, 255, 255);
            case 1:
                return new Color(255, 10, 10, 255);
            case 2:
                return new Color(255, 255, 10, 255);
            case 3:
                return new Color(50, 255, 10, 255);
            case 4:
                return new Color(10, 255, 255, 255);
            case 5:
                return new Color(10, 10, 255, 255);
            case 6:
                return new Color(255, 10, 255, 255);
        }
    }
}

public class StudiesOfTheInkblotOrbiterProj : ModProjectile {
    public override String Texture => ModContent.GetInstance<StudiesOfTheInkblotProj>().Texture;

    public override void SetStaticDefaults() {
        Main.projFrames[Projectile.type] = 7;
    }

    public override void SetDefaults() {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 80;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.scale = 0.8f;
        Projectile.extraUpdates = 4;
        Projectile.penetrate = -1;
    }

    public override Boolean? CanDamage() {
        return false;
    }

    public override Color? GetAlpha(Color lightColor) {
        return StudiesOfTheInkblotProj.FrameToColor(Projectile.frame);
    }

    public static Vector2 GetPosition(Single X, Single Y, Single T, Vector2 playerOrigin) {
        return playerOrigin + new Vector2((Single)Math.Sin(T) * X, (Single)Math.Cos(T) * Y);
    }

    public static Int32 X = 100;
    public static Int32 Y = 215;

    public static void Spawn4(EntitySource_ItemUse_WithAmmo source, Int32 plr) {
        var center = Main.player[plr].Center;
        Int32 damage = Main.player[plr].HeldItem.damage;
        Single kb = Main.player[plr].HeldItem.knockBack;
        Int32 type = ModContent.ProjectileType<StudiesOfTheInkblotOrbiterProj>();
        Int32 p = Projectile.NewProjectile(source, center, Vector2.Zero, type, damage, kb, plr);
        Main.projectile[p].localAI[0] = X;
        Main.projectile[p].localAI[1] = Y;
        p = Projectile.NewProjectile(source, center, Vector2.Zero, type, damage, kb, plr);
        Main.projectile[p].localAI[1] = X;
        Main.projectile[p].localAI[0] = Y;
        p = Projectile.NewProjectile(source, center, Vector2.Zero, type, damage, kb, plr, MathHelper.Pi);
        Main.projectile[p].localAI[0] = X;
        Main.projectile[p].localAI[1] = Y;
        p = Projectile.NewProjectile(source, center, Vector2.Zero, type, damage, kb, plr, MathHelper.Pi);
        Main.projectile[p].localAI[1] = X;
        Main.projectile[p].localAI[0] = Y;
    }

    public override void AI() {
        Int32 time = (Int32)Main.GameUpdateCount / 120;
        if (Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<StudiesOfTheInkblot>())
            Projectile.timeLeft = 4;

        if (Main.myPlayer == Projectile.owner) {
            if ((Int32)Projectile.localAI[0] == 0) {
                for (Int32 i = 0; i < Main.maxProjectiles; i++) {
                    if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner) {
                        if (i % 2 == 1) {
                            Projectile.localAI[0] = 215f;
                            Projectile.localAI[1] = 100f;
                        }
                        else {
                            Projectile.localAI[0] = 100f;
                            Projectile.localAI[1] = 215f;
                        }
                    }
                }
            }
            Projectile.Center = Main.player[Projectile.owner].Center + new Vector2((Single)Math.Sin(Projectile.ai[0]) * Projectile.localAI[0], (Single)Math.Cos(Projectile.ai[0]) * Projectile.localAI[1]);
        }
        if (Projectile.localAI[0] == Y) {
            time += 120;
        }
        Projectile.frame = time % 7;
        Projectile.ai[0] += 0.01f;
        var color = Projectile.GetAlpha(default(Color)) * Projectile.scale;
        if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(4)) {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, 0f, 0f, 0, color);
            d.noGravity = true;
        }
        Lighting.AddLight(Projectile.Center, color.ToVector3());
    }

    public override Boolean PreDraw(ref Color lightColor) {
        if (Projectile.alpha > 255) {
            return false;
        }
        Single colorMultiplier = 1 - Projectile.alpha / 255f;
        Single colorMultiplierSquared = colorMultiplier * colorMultiplier;
        var texture = TextureAssets.Projectile[Type].Value;
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        var spotlight = AequusTextures.Bloom;
        Main.spriteBatch.Draw(spotlight, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * colorMultiplierSquared, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.6f, SpriteEffects.None, 0f);
        var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, new Color(255, 255, 255, 255) * colorMultiplierSquared, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}