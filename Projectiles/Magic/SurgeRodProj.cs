using Aequus;
using Aequus.Common.Effects;
using Aequus.Common.Net.Sounds;
using Aequus.Common.Preferences;
using Aequus.Common.Graphics;
using Aequus.Common.Rendering;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic {
    public class SurgeRodProj : ModProjectile
    {
        public const int LightningCheckRate = 12;
        public const int Amount = 6;

        public static float DrawOpacity;
        public static bool DrawLightning;

        private TrailRenderer prim;
        private TrailRenderer thunderPrim;
        private TrailRenderer thunderBloomPrim;

        public int lightningCheck;

        public Color lightningBloomColor = new Color(222, 80, 30, 50);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
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
            Lighting.AddLight(Projectile.Center, new Vector3(0.75f, 0.75f, 0.1f));
            if ((int)Projectile.ai[0] == -2 || Projectile.ai[0] > 0f)
            {
                if (Projectile.localAI[1] < 3f)
                {
                    Projectile.localAI[1]++;
                }
                else
                {
                    Projectile.localAI[0] = Main.rand.Next(255);
                    Projectile.localAI[1] = 0f;
                }
                Projectile.velocity *= 0.2f;
                if (Projectile.frame < 5)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 4)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                    }
                }
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
                if (Projectile.localAI[1] == 0f)
                {
                    Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    Projectile.localAI[1]++;
                }
                Projectile.rotation += 0.1f;
                if ((int)Projectile.ai[0] == -1)
                {
                    if (Projectile.timeLeft < 2)
                    {
                        Projectile.timeLeft = 10800;
                        Projectile.ai[0] = -2f;
                        Projectile.netUpdate = true;
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
            Projectile.CollideWithOthers(0.2f);
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
            else if (Main.rand.NextBool((int)Math.Max(20 - Projectile.Distance(Main.projectile[surge].Center) / 32, 2)))
            {
                Dust.NewDustPerfect(Vector2.Lerp(Projectile.Center, Main.projectile[surge].Center, Main.rand.NextFloat(1f)), ModContent.DustType<MonoDust>(), newColor: new Color(255, 200, 128, 135), Scale: Main.rand.NextFloat(1f, 2f));
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
            DrawLightning = true;
            CheckPrims();
            PixelizationScreenRenderer.PrepareRender("SurgeRodProj", (sb) =>
            {
                thunderPrim.drawOffset = Vector2.Zero;
                thunderPrim.WorldTrail = false;
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
            });
            if (prim == null)
            {
                prim = new TrailRenderer(TrailTextures.Trail[2].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(12f) * (1f - p), (p) => new Color(255, 50, 10, 200) * (float)Math.Pow(1f - p, 2f))
                {
                    drawOffset = Projectile.Size / 2f
                };
            }
            prim.Draw(Projectile.oldPos);
            //for (int i = 0; i < trailLength; i++)
            //{
            //    float progress = 1f - 1f / trailLength * i;
            //    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, new Color(188, 128, 10, 10) * progress * opacity, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0f);
            //}
            foreach (var v in Helper.CircularVector(4, Projectile.rotation))
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
            GenerateAndDrawLightning(Projectile.localAI[0], difference, screenPosition);
            if (Aequus.HQ)
            {
                GenerateAndDrawLightning(Projectile.localAI[0] * 2f, difference, screenPosition);
                GenerateAndDrawLightning(Projectile.localAI[0] * 4f, difference, screenPosition);
            }
            //var normal = Vector2.Normalize(difference);
            //float length = Main.rand.NextFloat(difference.Length());
        }
        private void CheckPrims()
        {
            if (thunderPrim == null)
            {
                thunderPrim = new ForceCoordTrailRenderer(TrailTextures.Trail[1].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(4f), (p) => new Color(255, 180, 160, 40) * (float)Math.Pow(DrawOpacity, 2f), obeyReversedGravity: false, worldTrail: false)
                { coord1 = 1f, coord2 = 0f, };
            }
            if (thunderBloomPrim == null)
            {
                thunderBloomPrim = new ForceCoordTrailRenderer(TrailTextures.Trail[3].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(22f), (p) => lightningBloomColor * (float)Math.Pow(DrawOpacity, 2f), obeyReversedGravity: false, worldTrail: false)
                { coord1 = 1f, coord2 = 0f, };
            }
        }
        private void GenerateAndDrawLightning(float timer, Vector2 difference, Vector2 screenPosition)
        {
            var coordinates = GenerateLightningCoords(timer, difference, screenPosition);
            if (coordinates == null)
            {
                return;
            }
            //for (int i = 0; i < 5 && i < coordinates.Length; i++)
            //    AequusHelpers.dustDebug(coordinates[i] + Main.screenPosition);
            //if (Main.LocalPlayer.gravDir == -1)
            //{
            //    AequusHelpers.ScreenFlip(coordinates);
            //}
            DrawLightningPrim(coordinates);
        }
        private void DrawLightningPrim(Vector2[] coordinates)
        {
            thunderPrim.Draw(coordinates);
            thunderBloomPrim.Draw(coordinates, Main.GlobalTimeWrappedHourly * 0.7f, 1.2f);
        }
        private Vector2[] GenerateLightningCoords(float timer, Vector2 difference, Vector2 screenPosition)
        {
            if (difference.HasNaNs() || difference.Length() < 25f)
            {
                return null;
            }
            int amt = (int)Math.Max(difference.Length() / (ClientConfig.Instance.HighQuality ? 20 : 60), 6);
            Vector2[] coordinates = new Vector2[amt];
            var rand = LegacyEffects.EffectRand;
            int old = rand.SetRand((int)timer);
            float intensity = 11f;
            for (int i = 0; i < coordinates.Length; i++)
            {
                var offset = new Vector2(rand.Rand(-intensity, intensity), rand.Rand(-intensity, intensity));
                coordinates[i] = difference / (coordinates.Length - 1) * i + screenPosition + offset;
                if (i > 0)
                    coordinates[i] = Vector2.Lerp(coordinates[i], coordinates[i - 1], 0.15f);
            }
            rand.SetRand(old);
            return coordinates;
        }

        public static void DrawResultTexture()
        {
            if (!DrawLightning)
                return;

            DrawLightning = false;
            if (PixelizationScreenRenderer.TryGetResult("SurgeRodProj", out var texture))
            {
                Main.spriteBatch.Draw(texture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), texture.Bounds, Color.White, 0f, Vector2.Zero,
                    SpriteEffects.None, 0f);
            }
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

        public override void SetStaticDefaults()
        {
            AequusProjectile.InflictsHeatDamage.Add(Type);
        }

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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.IsTheDestroyer())
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ModContent.GetInstance<SurgeRodHitSound>().Play(target.Center);
            target.AddBuff(BuffID.OnFire3, 480);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ModContent.GetInstance<SurgeRodHitSound>().Play(target.Center);
            target.AddBuff(BuffID.OnFire, 480);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.DeathrayHitbox(Projectile.Center, targetHitbox, Projectile.ai[0], Projectile.ai[1], Projectile.width);
        }
    }

    public class SurgeRodHitSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.electricHit with { Volume = 0.5f, PitchVariance = 0.2f };
        }
    }
}