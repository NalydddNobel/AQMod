using Aequus.Items.Equipment.Accessories.CrownOfBlood.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.CrownOfBlood {
    public class BoneHelmMinion : ModProjectile {
        protected Vector2 _startingPoint;
        public int auraRange;

        private float GetAuraScale() {
            return MathF.Pow(Projectile.ai[1] / 120f, 2f);
        }

        public override void SetStaticDefaults() {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
        }

        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 3400;
            _startingPoint = default;
            auraRange = 250;
        }

        public override bool? CanCutTiles() {
            return false;
        }

        public override void AI() {
            if (_startingPoint == default) {
                _startingPoint = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 320f;
            }
            if (Projectile.timeLeft < 255) {
                Projectile.alpha++;
                Projectile.ai[1] -= 2f;
                if (Projectile.alpha > 255) {
                    Projectile.alpha = 255;
                }
                Projectile.velocity += Vector2.Normalize(_startingPoint - Projectile.Center) * 0.066f;
            }
            else if (Projectile.alpha > 0) {
                Projectile.alpha -= 5;
                if (Projectile.alpha <= 0) {
                    Projectile.alpha = 0;
                }
            }
            Projectile.ai[0]++;
            float speed = Projectile.velocity.Length();
            if (Projectile.velocity != Vector2.Zero) {
                if (Projectile.timeLeft > 255) {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }

                Projectile.velocity *= 0.996f;
                if (Projectile.ai[1] > 0f || !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
                    if (speed < 2f) {
                        Projectile.velocity *= 0.99f;
                        if (Projectile.ai[1] < 1f) {
                            Projectile.ai[1] = 1f;
                        }
                    }
                }
                if (speed < 0.05f) {
                    Projectile.velocity = Vector2.Zero;
                }
            }

            var lightColor = Color.Violet.ToVector3() * 0.05f;
            DelegateMethods.v3_1 = lightColor;
            Utils.PlotTileLine(Projectile.Center, _startingPoint, 128f, DelegateMethods.CastLight);

            if (speed < 1f) {
                float scale = GetAuraScale();

                for (int i = 0; i < 100; i++) {
                    var offset = (i / 30f * MathHelper.TwoPi).ToRotationVector2() * auraRange * scale;
                    Lighting.AddLight(Projectile.Center + offset, lightColor * 2f);
                }
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (!Main.npc[i].active || Main.npc[i].townNPC || Main.npc[i].IsProbablyACritter() || Projectile.Distance(Main.npc[i].Center) > auraRange) {
                        continue;
                    }

                    Main.npc[i].AddBuff(ModContent.BuffType<BoneHelmMinionDebuff>(), 30, quiet: true);
                }
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 120f) {
                    Projectile.ai[1] = 120f;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out _);
            origin.Y /= 10f;
            var drawPosition = (Projectile.position + offset).Floor();
            var n = Vector2.Normalize(drawPosition - _startingPoint);
            float speed = Projectile.velocity.Length();
            var minionDrawPosition = drawPosition;
            if (speed < 1f) {
                minionDrawPosition += n * MathF.Sin(Projectile.ai[0] * 0.033f) * 10f * (1f - speed);
            }
            float startingPointDistance = Vector2.Distance(drawPosition, _startingPoint) / Projectile.scale;
            float opacity = MathF.Pow(Projectile.Opacity, 2f);

            var color = Color.Violet * 0.33f * opacity;
            var spinningPoint = Projectile.rotation.ToRotationVector2();
            if (Projectile.ai[1] > 1f) {
                var auraTexture = AequusTextures.GoreNestAura;
                float auraScaleMultiplier = auraRange * 2f / auraTexture.Width;
                float scale = GetAuraScale();
                Main.EntitySpriteDraw(
                    AequusTextures.Bloom0,
                    drawPosition - Main.screenPosition,
                    null,
                    Color.Black * 0.8f * scale,
                    0f,
                    AequusTextures.Bloom0.Size() / 2f,
                    scale * 2f,
                    SpriteEffects.None,
                    0
                );

                var auraColor = color with { A = 0 } * 0.2f;
                Main.EntitySpriteDraw(
                    auraTexture,
                    drawPosition - Main.screenPosition,
                    null,
                    auraColor * scale,
                    0f,
                    auraTexture.Size() / 2f,
                    scale * auraScaleMultiplier,
                    SpriteEffects.None,
                    0
                );
                for (int i = 0; i < 4; i++) {
                    double radians = Projectile.ai[0] / 30f * MathHelper.TwoPi + MathHelper.PiOver2 * i;
                    Main.EntitySpriteDraw(
                        auraTexture,
                        drawPosition - Main.screenPosition + spinningPoint.RotatedBy(radians) * 2f,
                        null,
                        auraColor * scale,
                        0f,
                        auraTexture.Size() / 2f,
                        scale * auraScaleMultiplier,
                        SpriteEffects.None,
                        0
                    );
                }

            }

            for (int i = 0; i < 4; i++) {
                double radians = Projectile.ai[0] / 30f * MathHelper.TwoPi + MathHelper.PiOver2 * i;
                Main.EntitySpriteDraw(
                    texture,
                    minionDrawPosition - Main.screenPosition + spinningPoint.RotatedBy(radians) * 6f,
                    frame,
                    color,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(
                texture,
                minionDrawPosition - Main.screenPosition,
                frame,
                Color.Black * opacity,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            return false;
        }
    }
}