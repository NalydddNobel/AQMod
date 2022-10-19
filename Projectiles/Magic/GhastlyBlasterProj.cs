using Aequus;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class GhastlyBlasterProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.manualDirectionChange = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 10;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(120);
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];

            float progress = 1f - Projectile.ai[1] / (player.itemAnimationMax * 2f);
            var aequus = player.Aequus();
            if (progress < 0f)
            {
                if (aequus.itemCombo > 0)
                {
                    aequus.itemCombo = 0;
                }
                else
                {
                    aequus.itemCombo = (ushort)(player.itemAnimationMax * 4);
                }
                Projectile.Kill();
                return;
            }

            if (Projectile.ai[1] == 0f)
            {
                Projectile.direction = aequus.itemCombo > 0 ? -1 : 1;
                AequusHelpers.CappedMeleeScale(Projectile);
                Projectile.netUpdate = true;
                Projectile.ai[1] += 1f;
            }

            if (Projectile.numUpdates == -1)
            {
                if (Projectile.frame < 9 && (Projectile.frameCounter % 2 == 0 || Projectile.frame < 4))
                {
                    Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                    if (Projectile.frame == 7)
                    {
                        var s = SoundID.Item122;
                        s.PitchVariance = 0.1f;
                        SoundEngine.PlaySound(s, Projectile.Center);
                    }
                }
                Projectile.frameCounter++;
            }
            float speedMultiplier = Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee);
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.position.X = playerCenter.X - Projectile.width / 2;
            Projectile.position.Y = playerCenter.Y - Projectile.height / 2;
            if (!player.frozen && !player.stoned)
            {
                Projectile.ai[1] += 1f / (Projectile.extraUpdates + 1);
                Projectile.ai[0] = (float)Math.Sin(SwordProjectileBase.GenericSwing1(progress, 2f) * MathHelper.Pi);
            }
            float p = 0f;
            if (progress < 0.2f)
            {
                p = 1f - progress / 0.2f;
            }
            else if (progress > 0.8f)
            {
                p = (progress - 0.8f) / 0.2f;
            }
            Projectile.Opacity = 1f - p;
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            var dir = Vector2.Normalize(Projectile.velocity.RotatedBy((float)Math.Sin(progress - 0.5f) * 1.33f * Projectile.direction)).UnNaN();
            Projectile.position += dir * (Projectile.ai[0] * 30f + 15f);
            Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = dir.ToRotation() + (Projectile.spriteDirection == 1 ? MathHelper.Pi : 0f);
            player.ChangeDir(-Projectile.spriteDirection);

            Projectile.localAI[0] = ScanLaser(dir);
            Projectile.localAI[1] = dir.ToRotation();

            if (progress > 0.25f && progress < 0.75f)
            {
                var endPoint = Projectile.Center + dir * Projectile.localAI[0];
                if (Main.rand.NextBool(Projectile.extraUpdates / 3 + 1))
                {
                    var d = Dust.NewDustDirect(endPoint - Projectile.Size / 2f, Projectile.width, Projectile.height, DustID.AncientLight, -dir.X * 2f, -dir.Y * 2f, 128, Color.White, 0.2f + 1f * (float)Math.Pow(Projectile.ai[0], 2f));
                    d.velocity *= 0.45f;
                    d.scale *= 2.45f;
                    d.noGravity = true;
                }
            }
        }

        public float ScanLaser(Vector2 dir)
        {
            float[] laserScanResults = new float[50];
            Collision.LaserScan(Projectile.Center, dir, 8f * Projectile.scale, 1200f, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= laserScanResults.Length;

            return averageLengthSample;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.localAI[1].ToRotationVector2() * Projectile.localAI[0];
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, 16f * Projectile.scale, ref _);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawColor = Projectile.GetAlpha(lightColor);
            var swordTip = Projectile.Center;
            var frame = Projectile.Frame();
            var origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            var effects = Projectile.GetSpriteEffect();

            //foreach (var v in AequusHelpers.CircularVector(4, Main.GlobalTimeWrappedHourly))
            //{
            //    Main.EntitySpriteDraw(texture, swordTip + v * 2f - Main.screenPosition, frame, new Color(10, 60, 100, 0), Projectile.rotation, origin, Projectile.scale, effects, 0);
            //}

            DrawLaser();
            Main.EntitySpriteDraw(texture, swordTip - Main.screenPosition, frame, drawColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }

        public void DrawLaser()
        {
            float progress = 1f - Projectile.ai[1] / (Main.player[Projectile.owner].itemAnimationMax * 2f);
            var dir = Vector2.Normalize(Projectile.velocity.RotatedBy((float)Math.Sin(progress - 0.5f) * 1.33f * Projectile.direction)).UnNaN();

            var startPosition = Projectile.Center - Main.screenPosition + dir * 46f;
            var endPosition = Projectile.Center + dir * Projectile.localAI[0] - Main.screenPosition;
            float scale = Projectile.Opacity * Projectile.scale;
            var color = new Color(150, 180, 255, 128) * scale;

            AequusHelpers.DrawLine(startPosition + dir * 18f * scale, endPosition, 16f * scale, color);
            float rotation = dir.ToRotation() - MathHelper.PiOver2;
            var texture = ModContent.Request<Texture2D>(Texture + "Laser", AssetRequestMode.ImmediateLoad).Value;
            var frame = texture.Frame(verticalFrames: 3, frameY: 0);
            var origin = new Vector2(frame.Width / 2f, 6f);
            Main.spriteBatch.Draw(texture, startPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
            float segmentBit = (frame.Height / 2f + 4f) * scale;
            int segments = (int)((startPosition - endPosition).Length() / segmentBit);
            frame = frame.Frame(0, 1);
            ScreenCulling.SetPadding(100);
            for (int i = 0; i < segments; i++)
            {
                var drawCoords = startPosition + dir * segmentBit * i;
                if (!ScreenCulling.OnScreen(drawCoords))
                    return;
                Main.spriteBatch.Draw(texture, drawCoords, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, endPosition - dir * frame.Height / 2f * scale, frame.Frame(0, 1), color, rotation, origin, scale, SpriteEffects.None, 0f);

        }
    }
}