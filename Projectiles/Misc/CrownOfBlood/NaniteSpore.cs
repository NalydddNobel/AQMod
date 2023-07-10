using Aequus.Common.Graphics;
using Aequus.Content;
using Aequus.Projectiles.Misc.SporeSac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.CrownOfBlood {
    public class NaniteSpore : SporeSacSentry {
        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width *= 2;
            Projectile.height *= 2;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft *= Projectile.MaxUpdates;
            _lightColor = Color.Blue.ToVector3();
        }

        public override void AI() {
            for (int i = Projectile.oldRot.Length - 1; i > 0; i--) {
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }
            Projectile.oldRot[0] = Projectile.velocity.ToRotation();
            base.AI();
            if (Projectile.ai[2] == 0) {
                Projectile.ai[2] = Main.rand.NextFloat(-1f, 1f);
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[2] * 0.5f);
            Projectile.ai[2] *= 0.97f;
            Projectile.rotation += 0.1f;
            if (Projectile.velocity.Length() < 4f) {
                Projectile.velocity *= 1.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.Confused, 600);
        }

        public override void Kill(int timeLeft) {
            if (Main.netMode == NetmodeID.Server || Projectile.penetrate != 0) {
                return;
            }

            if (Main.myPlayer == Projectile.owner) {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<NaniteExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
            for (int i = 0; i < 10; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            var drawCoordinates = Projectile.position + off - Main.screenPosition;
            Main.spriteBatch.Draw(AequusTextures.NaniteSpore, drawCoordinates, null, Projectile.GetAlpha(lightColor), Projectile.rotation, AequusTextures.NaniteSpore.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);

            float flareScale = Helper.Wave(Main.GlobalTimeWrappedHourly * 30f + Projectile.whoAmI * 2f, 0.7f, 1f);
            var flareColor = Color.CornflowerBlue with { A = 0 } * Projectile.Opacity * flareScale;
            Main.spriteBatch.Draw(AequusTextures.Flare, drawCoordinates, null, flareColor, 0f, AequusTextures.Flare.Size() / 2f, new Vector2(0.3f, 0.66f) * Projectile.scale * flareScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(AequusTextures.Flare, drawCoordinates, null, flareColor, MathHelper.PiOver2, AequusTextures.Flare.Size() / 2f, new Vector2(0.45f, 1f) * Projectile.scale * flareScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World();

            var trailStartColor = Color.Cyan * Projectile.Opacity;
            var trailEndColor = Color.Blue with { A = 0 } * Projectile.Opacity;

            AequusDrawing.ApplyBasicEffect(AequusTextures.Pixel);

            AequusDrawing.VertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot,
                p => Color.Lerp(trailStartColor, trailEndColor, 1f - MathF.Pow(1f - p, 2f)) * (1f - p),
                p => 2f,
                -Main.screenPosition + Projectile.Size / 2f, true, true);

            AequusDrawing.VertexStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World();
            return false;
        }
    }
}