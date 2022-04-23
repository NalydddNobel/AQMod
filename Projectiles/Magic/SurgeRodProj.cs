using Aequus.Common.Configuration;
using Aequus.Effects;
using Aequus.Effects.Prims;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class SurgeRodProj : ModProjectile
    {
        private LegacyPrimRenderer prim;
        private LegacyPrimRenderer bloomPrim;

        public Color lightningBloomColor = new Color(128, 30, 10, 0);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 11;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == -2 || Projectile.ai[0] > 0f)
            {
                Projectile.rotation = 0f;
                Projectile.velocity *= 0.2f;
                if (Projectile.timeLeft > 60)
                {
                    if ((int)Projectile.ai[0] == -2)
                    {
                        int surge = FindClosestSurge();
                        if (surge != -1)
                        {
                            Projectile.ai[0] = surge + 1;
                        }
                    }
                    else
                    {
                        UpdateSurge(0);
                    }
                    if ((int)Projectile.ai[1] == 0 || (int)Projectile.ai[1] == -2)
                    {
                        int surge = FindClosestSurge();
                        if (surge != -1)
                        {
                            Projectile.ai[1] = surge + 1;
                        }
                    }
                    else
                    {
                        UpdateSurge(1);
                    }

                    if (Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] > 3)
                    {
                        bool imOldest = true;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].timeLeft < Projectile.timeLeft
                                && ((int)Main.projectile[i].ai[0] != -1 || (int)Main.projectile[i].ai[0] != 0))
                            {
                                imOldest = false;
                                break;
                            }
                        }
                        if (imOldest)
                        {
                            Projectile.timeLeft = 60;
                        }
                    }
                }
                else
                {
                    Projectile.alpha -= 4;
                }
            }
            else
            {
                if ((int)Projectile.ai[0] == -1)
                {
                    if (Projectile.timeLeft < 2)
                    {
                        Projectile.timeLeft = 10800;
                        Projectile.ai[0] = -2f;
                        Projectile.frame = 4;
                        Projectile.scale = 1f;
                    }
                    else if (Projectile.timeLeft < 8)
                    {
                        Projectile.scale += 0.12f;
                    }
                }
                if (Projectile.frame != 4)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 4)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 3)
                            Projectile.frame = 0;
                    }
                }
                if (Projectile.timeLeft < 17)
                {
                    if ((int)Projectile.ai[0] != -1)
                    {
                        var endLocation = Projectile.position + Projectile.velocity * 16f;
                        Projectile.velocity = (endLocation - Projectile.position) / 16f;
                    }
                    Projectile.ai[0] = -1f;
                }
            }
        }
        private int FindClosestSurge()
        {
            int result = -1;
            float resultLength = 500f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].type == Projectile.type &&
                    ((int)Main.projectile[i].ai[0] == -2 || (int)Main.projectile[i].ai[1] == 0f || (int)Main.projectile[i].ai[1] == -2) && Main.projectile[i].timeLeft > 60)
                {
                    if (i == (int)Projectile.ai[0] - 1 || i == (int)Projectile.ai[1] - 1 || (Main.projectile[i].ai[0] - 1) == Projectile.whoAmI || (Main.projectile[i].ai[1] - 1) == Projectile.whoAmI)
                    {
                        continue;
                    }
                    float length = Projectile.Distance(Main.projectile[i].Center);
                    if (length < resultLength)
                    {

                        result = i;
                        resultLength = length;
                    }
                }
            }
            return result;
        }
        private void UpdateSurge(int aiIndex)
        {
            int surge = (int)Projectile.ai[aiIndex] - 1;
            if (!Main.projectile[surge].active || Main.projectile[surge].type != Projectile.type)
            {
                Projectile.ai[aiIndex] = -2f;
                return;
            }
            //var difference = Main.projectile[surge].Center - Projectile.Center;
            //var normal = Vector2.Normalize(difference);
            //float length = Main.rand.NextFloat(difference.Length());
            //var d = Dust.NewDustPerfect(Projectile.Center + normal * length, ModContent.DustType<MonoDust>(), null, 0, new Color(225, 100, 10, 10));
            //if (Main.rand.NextBool())
            //{
            //    d.velocity = normal.RotatedBy(MathHelper.PiOver2) * 3f * (float)Math.Sin(length * 0.4f + Main.GlobalTimeWrappedHourly * 0.1f + Projectile.position.X + Projectile.position.Y);
            //}
            //else
            //{
            //    d.velocity *= Main.rand.NextFloat(0.1f, 0.75f);
            //}
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = Projectile.Opacity;
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            var origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            var color = Projectile.GetAlpha(lightColor);
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            if ((int)Projectile.ai[0] == -2 || Projectile.ai[0] > 0f)
            {
                if ((int)Projectile.ai[0] != -2)
                {
                    DrawSurge((int)Projectile.ai[0] - 1);
                }
                if ((int)Projectile.ai[1] != -2 && (int)Projectile.ai[1] != 0)
                {
                    DrawSurge((int)Projectile.ai[1] - 1);
                }
            }
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f - 1f / trailLength * i;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, new Color(188, 128, 10, 10) * progress * opacity, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            foreach (var v in AequusHelpers.CircularVector(4, Projectile.rotation))
            {
                Main.spriteBatch.Draw(texture, Projectile.position + v * 2f + offset - Main.screenPosition, frame, new Color(128, 128, 10, 10) * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, color * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        private void DrawSurge(int to)
        {
            var difference = Main.projectile[to].Center - Projectile.Center;
            var screenPosition = Projectile.Center - Main.screenPosition;

            CheckPrims();
            GenerateAndDrawLightning(Main.GlobalTimeWrappedHourly * 30f, difference, screenPosition);
            if (Aequus.HQ)
            {
                GenerateAndDrawLightning(Main.GlobalTimeWrappedHourly * 60f, difference, screenPosition);
                GenerateAndDrawLightning(Main.GlobalTimeWrappedHourly * 80f, difference, screenPosition);
            }
            //var normal = Vector2.Normalize(difference);
            //float length = Main.rand.NextFloat(difference.Length());
        }
        private void CheckPrims()
        {
            if (prim == null)
            {
                prim = new LegacyPrimRenderer(Aequus.MyTex("Assets/Effects/Prims/ThickTrail"), LegacyPrimRenderer.DefaultPass,
                    (p) => new Vector2(2f), (p) => new Color(255, 180, 160, 40) * Projectile.Opacity, obeyReversedGravity: false, worldTrail: false);
            }
            if (bloomPrim == null)
            {
                bloomPrim = new LegacyPrimRenderer(Aequus.MyTex("Assets/Effects/Prims/ThickTrail"), LegacyPrimRenderer.DefaultPass,
                    (p) => new Vector2(8f), (p) => lightningBloomColor * Projectile.Opacity, obeyReversedGravity: false, worldTrail: false);
            }
        }
        private void GenerateAndDrawLightning(float timer, Vector2 difference, Vector2 screenPosition)
        {
            var coordinates = GenerateLightningCoords(timer, difference, screenPosition);
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(coordinates);
            }
            DrawLightningPrim(coordinates);
        }
        private void DrawLightningPrim(Vector2[] coordinates)
        {
            prim.Draw(coordinates);
            bloomPrim.Draw(coordinates);
        }
        private Vector2[] GenerateLightningCoords(float timer, Vector2 difference, Vector2 screenPosition)
        {
            Vector2[] coordinates = new Vector2[(ClientConfiguration.Instance.HighQuality ? 25 : 8)];
            int old = EffectsSystem.EffectRand.SetRand((int)timer / 2 * 2);
            for (int i = 0; i < coordinates.Length; i++)
            {
                var offset = Vector2.Lerp(new Vector2(EffectsSystem.EffectRand.Rand() / 15f, EffectsSystem.EffectRand.Rand() / 25f), new Vector2(EffectsSystem.EffectRand.Rand() / 15f, EffectsSystem.EffectRand.Rand() / 25f), timer / 2f % 1f);
                coordinates[i] = difference / coordinates.Length * i + screenPosition + offset;
            }
            EffectsSystem.EffectRand.SetRand(old);
            return coordinates;
        }


        //public override void Kill(int timeLeft)
        //{
        //    var center = Projectile.Center;
        //    for (int i = 0; i < 50; i++)
        //    {
        //        int d = Dust.NewDust(Projectile.position, 16, 16, ModContent.DustType<RedSpriteDust>());
        //        Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
        //    }
        //}
    }
}