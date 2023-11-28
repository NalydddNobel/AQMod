using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Armor.Castaway;

public class CastawayProj : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.SpikyBall);

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.timeLeft = 80;
        Projectile.tileCollide = false;
    }

    public override void AI() {
        if (Projectile.ai[0] == 0f) {
            Projectile.ai[0] = 1f;
            Projectile.timeLeft += Main.rand.Next(40);
        }
        Projectile.velocity *= 0.9f;
        Projectile.rotation += Projectile.velocity.X * 0.33f;

        if (Projectile.timeLeft == 10) {
            SoundEngine.PlaySound(SoundID.Item158, Projectile.Center);
        }

        int damage = Projectile.timeLeft < 10 ? Projectile.damage : 0;
        if (Main.rand.NextBool(Projectile.timeLeft / 10 + 1) && Projectile.timeLeft > 6) {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f, 24f), DustID.Electric, Scale: Main.rand.NextFloat(0.2f, 0.66f));
            d.noGravity = true;
            d.velocity *= 0.5f * d.scale;

            float closestDistance = float.MaxValue;
            int closestProjectile = -1;
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (!Main.projectile[i].active || Main.projectile[i].type != Type || i == Projectile.whoAmI) {
                    continue;
                }

                float distance = Projectile.Distance(Main.projectile[i].Center) + Main.rand.NextFloat(480f);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestProjectile = i;
                }
            }

            if (closestProjectile != -1 && Main.myPlayer == Projectile.owner) {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Main.projectile[closestProjectile].Center - Projectile.Center), ModContent.ProjectileType<CastawayProjLaser>(), damage, Projectile.knockBack, Projectile.owner, Projectile.identity, Main.projectile[closestProjectile].identity);
            }

            if (!Main.rand.NextBool(3)) {
                return;
            }

            closestDistance = float.MaxValue;
            int closestNPC = -1;
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (!Main.npc[i].active || !Main.npc[i].CanBeChasedBy(Projectile)) {
                    continue;
                }

                float distance = Projectile.Distance(Main.npc[i].Center);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestNPC = i;
                }
            }

            if (closestNPC != -1 && Main.myPlayer == Projectile.owner) {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Main.npc[closestNPC].Center - Projectile.Center), ModContent.ProjectileType<CastawayProjLaser>(), damage, Projectile.knockBack, Projectile.owner, Projectile.identity, -closestNPC - Main.maxProjectiles);
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        return false;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var drawCoordinates = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(texture, drawCoordinates, null, Color.White with { A = 0 }, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        if (Projectile.timeLeft < 16) {
            float animation = 1f - MathF.Pow(Projectile.timeLeft / 16f, 3f);
            var flareTexture = AequusTextures.Flare2;
            float flareScale = Projectile.scale * MathF.Sin(animation * MathHelper.Pi) * 0.75f;
            var flareColor = new Color(100, 255, 255, 0) * animation;
            var flareOrigin = flareTexture.Size() / 2f;
            Main.EntitySpriteDraw(AequusTextures.BloomStrong, drawCoordinates, null, flareColor * 0.33f * flareScale, Projectile.rotation, AequusTextures.BloomStrong.Size()/2f, 1f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(flareTexture, drawCoordinates, null, flareColor, Projectile.rotation, flareOrigin, flareScale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(flareTexture, drawCoordinates, null, flareColor, Projectile.rotation + MathHelper.PiOver2, flareOrigin, flareScale, SpriteEffects.None, 0f);
        }
        return false;
    }
}