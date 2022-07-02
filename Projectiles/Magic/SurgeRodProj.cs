using Aequus;
using Aequus.Graphics;
using Aequus.Graphics.Primitives;
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
        public const int LightningCheckRate = 12;
        public const int Amount = 12;

        public static float DrawOpacity;

        private TrailRenderer prim;
        private TrailRenderer bloomPrim;

        public int lightningCheck;

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
            Projectile.timeLeft = 3600;
        }

        public override bool? CanDamage()
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

                    if (lightningCheck <= 0)
                    {
                        lightningCheck = LightningCheckRate;
                    }
                    else
                    {
                        lightningCheck--;
                    }

                    int amountActive = 0;
                    int oldest = Projectile.whoAmI;
                    int timeLeftComparison = Projectile.timeLeft;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == Type && Main.projectile[i].timeLeft > 60
                            && ((int)Main.projectile[i].ai[0] != -1 || (int)Main.projectile[i].ai[0] != 0))
                        {
                            if (Main.projectile[i].timeLeft < timeLeftComparison)
                            {
                                oldest = i;
                                timeLeftComparison = Main.projectile[i].timeLeft;
                            }
                            amountActive++;
                        }
                    }

                    if (amountActive > Amount)
                    {
                        Main.projectile[oldest].timeLeft = 60;
                        Projectile.netUpdate = true;
                        Main.projectile[oldest].netUpdate = true;
                    }
                }
                else
                {
                    Projectile.alpha += 4;
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
                        Projectile.netUpdate = true;
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
            float resultLength = 750f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].timeLeft > 60)
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

            if (lightningCheck <= 0)
            {
                var difference = Main.projectile[surge].Center - Projectile.Center;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SurgeRodHitbox>(), Projectile.damage, Projectile.knockBack, Projectile.owner,
                    difference.ToRotation(), difference.Length());
            }
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

            DrawOpacity = Main.projectile[to].Opacity * Projectile.Opacity;

            CheckPrims();
            GenerateAndDrawLightning(Main.GlobalTimeWrappedHourly * 10f, difference, screenPosition);
            if (Aequus.HQ)
            {
                GenerateAndDrawLightning(Main.GlobalTimeWrappedHourly * 15f, difference, screenPosition);
                GenerateAndDrawLightning(Main.GlobalTimeWrappedHourly * 20f, difference, screenPosition);
            }
            //var normal = Vector2.Normalize(difference);
            //float length = Main.rand.NextFloat(difference.Length());
        }
        private void CheckPrims()
        {
            if (prim == null)
            {
                prim = new TrailRenderer(TextureCache.Trail[1].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(2f), (p) => new Color(255, 180, 160, 40) * DrawOpacity, obeyReversedGravity: false, worldTrail: false);
            }
            if (bloomPrim == null)
            {
                bloomPrim = new TrailRenderer(TextureCache.Trail[1].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(8f), (p) => lightningBloomColor * DrawOpacity, obeyReversedGravity: false, worldTrail: false);
            }
        }
        private void GenerateAndDrawLightning(float timer, Vector2 difference, Vector2 screenPosition)
        {
            var coordinates = GenerateLightningCoords(timer, difference, screenPosition);
            if (coordinates == null)
            {
                return;
            }
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
            if (difference.HasNaNs() || difference.Length() < 25f)
            {
                return null;
            }
            Vector2[] coordinates = new Vector2[(ClientConfig.Instance.HighQuality ? 25 : 8)];
            var rand = AequusEffects.EffectRand;
            int old = rand.SetRand((int)timer / 2 * 2);
            for (int i = 0; i < coordinates.Length; i++)
            {
                var offset = Vector2.Lerp(new Vector2(rand.Rand() / 15f, rand.Rand() / 25f), new Vector2(rand.Rand() / 15f, rand.Rand() / 25f), timer / 2f % 1f);
                coordinates[i] = difference / (coordinates.Length - 1) * i + screenPosition + offset;
            }
            rand.SetRand(old);
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

    public class SurgeRodHitbox : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.timeLeft = 12;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.IsTheDestroyer())
            {
                damage = (int)(damage * 0.50f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 480);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return AequusHelpers.DeathrayHitbox(Projectile.Center, targetHitbox, Projectile.ai[0], Projectile.ai[1], Projectile.width);
        }
    }
}