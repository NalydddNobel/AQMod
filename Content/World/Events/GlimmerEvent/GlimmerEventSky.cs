using AQMod.Assets;
using AQMod.Common.Configuration;
using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Content.World.Events.GlimmerEvent
{
    public class GlimmerEventSky : AQSky
    {
        public const string Name = "AQMod:GlimmerEventSky";

        public static float _glimmerLight;
        private float _cloudAlpha;
        private static bool _active;

        public static BGStarite[] _starites;

        public class BGStarite
        {
            public int size;
            public float x;
            public float y;
            public float velocityX;
            public float velocityY;
            public float rot;

            public const int size0X = 2;
            public const int size0Frames = 6;
            public const int size0Y = 0;
            public const int size0Width = 2;
            public const int size0Height = 2;

            internal static Texture2D _texture;

            public void Update(float midScreenX, float midScreenY, float value = 0f)
            {
                switch (size)
                {
                    default:
                        {
                            Vector2 gotoPos = Vector2.Normalize(new Vector2(midScreenX - x, midScreenY - y)).RotatedBy(value) * 4f;
                            velocityX = MathHelper.Lerp(velocityX, gotoPos.X, 0.001f);
                            velocityY = MathHelper.Lerp(velocityY, gotoPos.Y, 0.001f);
                            x += velocityX;
                            y += velocityY;
                        }
                        break;

                    case 1:
                        {
                            Vector2 gotoPos = Vector2.Normalize(new Vector2(midScreenX - x, midScreenY - y)).RotatedBy(value) * 9f;
                            rot += new Vector2(velocityX, velocityY).Length() * 0.0157f;
                            velocityX = MathHelper.Lerp(velocityX, gotoPos.X, 0.001f);
                            velocityY = MathHelper.Lerp(velocityY, gotoPos.Y, 0.00125f);
                            x += velocityX;
                            y += velocityY;
                        }
                        break;

                    case 2:
                        {
                            rot += new Vector2(velocityX, velocityY).Length() * 0.0157f;
                            Vector2 gotoPos = Vector2.Normalize(new Vector2(midScreenX - x, midScreenY - y)).RotatedBy(value) * 18f;
                            velocityX = MathHelper.Lerp(velocityX, gotoPos.X, 0.00125f);
                            velocityY = MathHelper.Lerp(velocityY, gotoPos.Y, 0.002f);
                            x += velocityX;
                            y += velocityY;
                        }
                        break;

                    case 3:
                        {
                            rot += new Vector2(velocityX, velocityY).Length() * 0.0157f;
                            Vector2 gotoPos = Vector2.Normalize(new Vector2(midScreenX - x, midScreenY - y)).RotatedBy(value) * 24f;
                            velocityX = MathHelper.Lerp(velocityX, gotoPos.X, 0.00125f);
                            velocityY = MathHelper.Lerp(velocityY, gotoPos.Y, 0.002f);
                            x += velocityX;
                            y += velocityY;
                        }
                        break;
                }
            }

            public void Draw(UnifiedRandom rand)
            {
                switch (size)
                {
                    default:
                        {
                            int x = size0Width * rand.Next(size0Frames);
                            var frame = new Rectangle(x, size0Y, size0Width, size0Height);
                            var orig = new Vector2(1f, 1f);
                            Main.spriteBatch.Draw(_texture, new Vector2((int)this.x, (int)y), frame, Color.White, rot, orig, 1f, SpriteEffects.None, 0f);
                        }
                        break;

                    case 1:
                        {
                            var frame = new Rectangle(0, 4, 10, 10);
                            var orig = frame.Size() / 2f;
                            Main.spriteBatch.Draw(_texture, new Vector2((int)x, (int)y), frame, Color.White, rot, orig, 0.5f, SpriteEffects.None, 0f);
                        }
                        break;

                    case 2:
                        {
                            var frame = new Rectangle(0, 16, 14, 14);
                            var orig = frame.Size() / 2f;
                            Main.spriteBatch.Draw(_texture, new Vector2((int)x, (int)y), frame, Color.White, rot, orig, 0.33f, SpriteEffects.None, 0f);
                        }
                        break;

                    case 3:
                        {
                            var frame = new Rectangle(0, 52, 24, 22);
                            var orig = frame.Size() / 2f;
                            Main.spriteBatch.Draw(_texture, new Vector2((int)x, (int)y), frame, Color.White, rot, orig, 0.45f, SpriteEffects.None, 0f);
                        }
                        break;
                }
            }
        }

        public static class BackgroundAura
        {
            private static bool active;
            private static float transition;
            private static float transitionSpeed;

            public static Color AuraColoring;
            public static float Brightness;

            public static float AuroraMin;
            public static float AuroraMax;
            public static float AuroraMinSpeed;

            public static bool AuraStillVisible => (transition > 0.02f || active) && Brightness > 0f;

            public static void Activate(float transitionSpeed = 0.01f)
            {
                active = true;
                BackgroundAura.transitionSpeed = transitionSpeed;
            }

            public static void Deactivate(float transitionSpeed = 0.01f)
            {
                active = false;
                BackgroundAura.transitionSpeed = transitionSpeed;
            }

            internal static void UpdateAura()
            {
                var config = ModContent.GetInstance<StariteConfig>();
                Brightness = config.BackgroundBrightness;
                AuraColoring = config.AuraColoring;
                if (AuraStillVisible)
                {
                    if (transitionSpeed < 0.01f)
                    {
                        transitionSpeed = 0.01f;
                    }
                    if (active)
                    {
                        if (transition < 1f)
                        {
                            transition += transitionSpeed;
                            if (transition > 1f)
                            {
                                transition = 1f;
                            }
                        }

                        AuroraMin = (float)Math.Sin(Main.GlobalTime * 0.01f);
                        AuroraMax = (float)Math.Cos(Main.GlobalTime * 0.005f);

                        if (AuroraMin < 0.05f)
                        {
                            AuroraMin = 0.05f;
                        }
                        if (AuroraMin > 0.05f)
                        {
                            AuroraMin = 0.05f;
                        }
                        if (AuroraMax < AuroraMin + 0.1f)
                        {
                            AuroraMax = AuroraMin + 0.1f;
                        }
                        if (AuroraMax > AuroraMin + 0.3f)
                        {
                            AuroraMax = AuroraMin + 0.3f;
                        }
                    }
                    else
                    {
                        if (transition > 0f)
                        {
                            transition -= transitionSpeed;
                            if (transition < 0f)
                            {
                                transition = 0f;
                            }
                        }
                    }
                }
            }

            internal static void RenderAura()
            {
                var color = AuraColoring * transition * Brightness;
                var config = ModContent.GetInstance<StariteConfig>();
                Main.spriteBatch.End();
                BatcherMethods.Background.Begin(Main.spriteBatch, BatcherMethods.Shader);

                var effect = EffectCache.GlimmerEventBackground;
                effect.Parameters["intensity"].SetValue(2f);
                effect.Parameters["pulse"].SetValue((float)Math.Sin(Main.GlobalTime * 0.0628f) * (float)Math.Cos(Main.GlobalTime * 0.14f) * 2.1f);
                effect.Parameters["time"].SetValue(Main.GlobalTime);
                effect.Parameters["screenOrigin"].SetValue(new Vector2(0.5f, 0.8f));
                effect.Parameters["color"].SetValue(color.ToVector3());
                effect.Techniques[0].Passes["BackgroundEffectPass"].Apply();
                Main.instance.GraphicsDevice.Textures[1] = ModContent.GetTexture("Terraria/Misc/Noise");

                Main.spriteBatch.Draw(AQTextures.Pixel, new Rectangle(0, 200 + AQUtils.BGTop * 2, Main.screenWidth, Main.screenHeight), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                if (config.BackgroundAurora)
                    RenderAuroras(effect, config);

                Main.spriteBatch.End();
                BatcherMethods.Background.Begin(Main.spriteBatch, BatcherMethods.Regular);
            }

            private static void RenderAuroras(Effect effect, StariteConfig config)
            {
                Main.spriteBatch.End();
                BatcherMethods.Background.Begin(Main.spriteBatch, BatcherMethods.Shader);
                effect.Parameters["minRange"].SetValue((int)(AuroraMin * 150) / 150f);
                effect.Parameters["maxRange"].SetValue((int)(AuroraMax * 150) / 150f);
                effect.Parameters["time"].SetValue(Main.GlobalTime + Main.screenPosition.X / 800f);
                effect.Techniques[0].Passes["MagicalCurrentAuroraPass"].Apply();
                
                var color = Color.Lerp(AuraColoring, config.StariteProjectileColoring, ((float)Math.Sin(Main.GlobalTime) + 1f) * 2f);
                if (GlimmerEvent.stariteDiscoParty)
                {
                    color = Main.DiscoColor;
                }
                color *= 0.35f * transition * Brightness;
                color.A = 0;
                Main.spriteBatch.Draw(AQTextures.Pixel, new Rectangle(0, 40 + AQUtils.BGTop / 2, Main.screenWidth, Main.screenHeight), null,
                    color, 0f, Vector2.Zero, SpriteEffects.None, 0f) ;
            }

            internal static void RenderAuraOld()
            {
                int width = Main.screenWidth + 40;
                int height = Main.screenHeight + 40;
                var texture = AQTextures.Lights[LightTex.Spotlight66x66];
                float scaleX = width / texture.Width * 1.75f;
                float scaleY = height / texture.Height;
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                Color color = AuraColoring * transition * Brightness;
                float y = Main.screenHeight / 2f + (325f - Main.screenPosition.Y / 16f);
                float x = Main.screenWidth / 2f;
                Main.spriteBatch.Draw(texture, new Vector2(x, y), null, color, 0f, frame.Size() / 2f, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
                if (_glimmerLight > 0f)
                {
                    _glimmerLight -= 0.0125f;
                    if (_glimmerLight < 0f)
                        _glimmerLight = 0f;
                }
            }
        }

        public static class FallingStars
        {
            public static List<StarModule> stars;

            public class StarModule
            {
                public int parentIndex;
                public Star parent;

                public Vector2[] oldPos;
                public Vector2 position;
                public Vector2 normal;
                public float speedGainedOverTime;
                public Vector2 velocity;
                public int lifeTime;
                public int lifeTimeMax;
                private float _scale;
                private Vector2 _origin;

                public StarModule(int star, UnifiedRandom rand)
                {
                    parentIndex = star;
                    parent = Main.star[star];
                    position = Main.star[star].position;
                    speedGainedOverTime = rand.NextFloat(0.01f, 0.05f) * parent.scale;
                    normal = new Vector2(0f, 1f).RotatedBy(rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                    lifeTime = 360;
                    lifeTimeMax = lifeTime;
                    _scale = Main.star[star].scale;
                    oldPos = new Vector2[50];
                    _origin = Main.starTexture[parent.type].Size() / 2f;
                }

                /// <summary>
                /// Return true to remove this Star Module from the pool
                /// </summary>
                /// <returns></returns>
                public bool Update()
                {
                    if (Main.dayTime)
                        return true;
                    parent.twinkle = 0.01f;
                    lifeTime -= Main.dayRate;
                    if (lifeTime <= 0)
                    {
                        return true;
                    }
                    for (int i = 0; i < Main.dayRate; i++)
                    {
                        velocity += normal * speedGainedOverTime;

                        for (int j = 0; j < 5; j++)
                        {
                            position += velocity / 5f;
                            AQUtils.CyclePositions(oldPos, position);

                        }
                    }
                    return false;
                }

                public void Render()
                {
                    int timeExisting = lifeTimeMax - lifeTime;
                    var texture = Main.starTexture[parent.type];
                    for (int i = 0; i < oldPos.Length; i++)
                    {
                        if (oldPos[i] != Vector2.Zero)
                        {
                            var drawPosition2 = AQUtils.BackgroundStars.GetRenderPosition(oldPos[i]);
                            float progress = 1f / oldPos.Length * i;
                            Main.spriteBatch.Draw(texture, drawPosition2, null,
                                Color.Lerp(new Color(128, 128, 128, 0) * _scale, BackgroundAura.AuraColoring * 0.8f, 1f - progress) * (1f - progress), parent.rotation, _origin, _scale * 0.8f * (1f - progress), SpriteEffects.None, 0f);
                        }
                    }
                    var drawPosition = AQUtils.BackgroundStars.GetRenderPosition(position);
                    Main.spriteBatch.Draw(texture, drawPosition, null, new Color(255, 255, 255, 255) * _scale, parent.rotation, _origin, _scale, SpriteEffects.None, 0f);

                    if (ModContent.GetInstance<AQConfigClient>().EffectQuality >= 1f)
                    {
                        if (timeExisting > 11)
                        {
                            float glowIn = 1f;
                            if (timeExisting < 20)
                            {
                                glowIn = (timeExisting - 10) / 10f;
                            }
                            var spotlightTexture = AQTextures.Lights[LightTex.Spotlight15x15];
                            var spotlightOrigin = spotlightTexture.Size() / 2f;
                            float spotlightScale = 0.6f * _scale * velocity.Length() / 20f;

                            if (timeExisting > 40 && (_scale < 0.4f || timeExisting < 60))
                            {
                                float shimmer = timeExisting % 10 / 10f;
                                Main.spriteBatch.Draw(spotlightTexture, drawPosition, null, new Color(255, 255, 255, 255) * _scale * glowIn, parent.rotation, spotlightOrigin, new Vector2(shimmer, shimmer * 0.3f), SpriteEffects.None, 0f);
                                Main.spriteBatch.Draw(spotlightTexture, drawPosition, null, new Color(255, 255, 255, 255) * _scale * glowIn, parent.rotation, spotlightOrigin, new Vector2(shimmer * 0.3f, shimmer), SpriteEffects.None, 0f);
                                Main.spriteBatch.Draw(spotlightTexture, drawPosition, null, BackgroundAura.AuraColoring * _scale * glowIn, parent.rotation, spotlightOrigin, new Vector2(shimmer, shimmer * 0.3f) * 1.5f, SpriteEffects.None, 0f);
                                Main.spriteBatch.Draw(spotlightTexture, drawPosition, null, BackgroundAura.AuraColoring * _scale * glowIn, parent.rotation, spotlightOrigin, new Vector2(shimmer * 0.3f, shimmer) * 1.5f, SpriteEffects.None, 0f);
                            }

                            for (int i = 0; i < oldPos.Length; i++)
                            {
                                if (oldPos[i] != Vector2.Zero)
                                {
                                    var drawPosition2 = AQUtils.BackgroundStars.GetRenderPosition(oldPos[i]);
                                    float progress = 1f / oldPos.Length * i;
                                    Main.spriteBatch.Draw(spotlightTexture, drawPosition2, null,
                                        Color.Lerp(new Color(255, 255, 255, 0) * _scale, BackgroundAura.AuraColoring * 1.25f, 1f - progress) * (1f - progress) * glowIn, parent.rotation, spotlightOrigin, spotlightScale * (1f - progress), SpriteEffects.None, 0f);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool CanSpawnBGStarites()
        {
            return _starites == null && ModContent.GetInstance<StariteConfig>().BackgroundStarites && GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer) < GlimmerEvent.MaxDistance;
        }

        public static void InitNight()
        {
            var sky = (GlimmerEventSky)SkyManager.Instance[key: Name];
            var random = sky.rand;
            if (!GlimmerEvent.IsGlimmerEventCurrentlyActive())
            {
            }
            else
            {
                if (CanSpawnBGStarites())
                    SpawnBGStarites(random);
            }
        }

        public static void SpawnBGStarites(UnifiedRandom random)
        {
            float midX = Main.screenWidth / 2f;
            _starites = new BGStarite[150];
            for (int i = 0; i < 120; i++)
            {
                _starites[i] = new BGStarite();
                _starites[i].x = midX + random.Next(-120, 120);
                _starites[i].y = random.NextBool() ? Main.screenHeight + random.Next(20, 480) : random.Next(-480, -12);
                _starites[i].size = 0;
            }
            for (int i = 120; i < 135; i++)
            {
                _starites[i] = new BGStarite();
                _starites[i].x = midX + random.Next(-360, 360);
                _starites[i].y = random.NextBool() ? Main.screenHeight + random.Next(12, 480) : random.Next(-480, -12);
                _starites[i].size = 1;
            }
            for (int i = 135; i < 145; i++)
            {
                _starites[i] = new BGStarite();
                _starites[i].x = midX + random.Next(-Main.screenWidth, Main.screenWidth);
                _starites[i].size = 2;
            }
            for (int i = 135; i < 150; i++)
            {
                _starites[i] = new BGStarite();
                _starites[i].x = midX + random.Next(-Main.screenWidth, Main.screenWidth);
                _starites[i].size = 3;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_active)
            {
                BackgroundAura.Activate(transitionSpeed: 0.01f);
            }
            else
            {
                BackgroundAura.Deactivate(transitionSpeed: 0.01f);
            }
            int tileDistance = GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer);
            if (!Main.dayTime && ModContent.GetInstance<StariteConfig>().BackgroundStars)
            {
                if (FallingStars.stars == null)
                {
                    FallingStars.stars = new List<FallingStars.StarModule>();
                }

                int starFallChance = 10;
                if (tileDistance > GlimmerEvent.SuperStariteDistance)
                {
                    starFallChance = 128;
                }

                if (_active && rand.NextBool(starFallChance))
                {
                    int star = rand.Next(Main.numStars);
                    if (Main.star[star].twinkle > 0.8f && FallingStars.stars.Find((s) => star == s.parentIndex) == null)
                    {
                        FallingStars.stars.Add(new FallingStars.StarModule(star, rand));
                    }
                }

                if (FallingStars.stars.Count != 0)
                {
                    for (int i = 0; i < FallingStars.stars.Count; i++)
                    {
                        if (FallingStars.stars[i].Update())
                        {
                            FallingStars.stars.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else if (!_active)
                {
                    FallingStars.stars = null;
                }
            }
            else
            {
                FallingStars.stars = null;
            }
            if (_starites != null)
            {
                float midX = Main.screenWidth / 2f + (float)Math.Sin(Main.GlobalTime) * 125f;
                float midY = Main.screenHeight / 2f + (float)Math.Cos(Main.GlobalTime) * 125f;
                if (!_active || Main.gameMenu || tileDistance > GlimmerEvent.MaxDistance)
                {
                    for (int i = 0; i < 120; i++)
                    {
                        _starites[i].velocityY += -0.01f;
                    }
                    for (int i = 120; i < 135; i++)
                    {
                        _starites[i].velocityY += -0.0125f;
                    }
                    for (int i = 135; i < 145; i++)
                    {
                        _starites[i].velocityY += -0.015f;
                    }
                    for (int i = 145; i < 150; i++)
                    {
                        _starites[i].velocityY += -0.02f;
                    }
                    for (int i = 0; i < 150; i++)
                    {
                        if (_starites[i].y > -20f)
                        {
                            break;
                        }
                        if (i == 149)
                        {
                            _starites = null;
                            break;
                        }
                    }
                    if (_starites != null)
                    {
                        for (int i = 0; i < 150; i++)
                        {
                            _starites[i].velocityX *= 0.9f;
                            _starites[i].x += _starites[i].velocityX;
                            _starites[i].y += _starites[i].velocityY;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 120; i++)
                    {
                        _starites[i].Update(midX, midY, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.X / Main.screenPosition.Y + i * 0.0125f) + rand.NextFloat(0.06f, 0.18f));
                    }
                    for (int i = 120; i < 135; i++)
                    {
                        _starites[i].Update(midX, midY - Main.screenHeight / 2f, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.Y / Main.screenPosition.X + i * 0.12f) + rand.NextFloat(0.01f, 0.02f));
                    }
                    for (int i = 135; i < 145; i++)
                    {
                        _starites[i].Update(midX, midY - Main.screenHeight / 3f, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.Y * Main.screenPosition.X + i * 0.12f) + rand.NextFloat(0.01f, 0.02f));
                    }
                    for (int i = 145; i < 150; i++)
                    {
                        _starites[i].Update(midX, midY - Main.screenHeight / 5f, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.Y * Main.screenPosition.X + i) + rand.NextFloat(0.01f, 0.02f));
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (BGStarite._texture == null)
            {
                BGStarite._texture = ModContent.GetTexture("AQMod/Assets/Backgrounds/Entities/Starite");
            }
            if (maxDepth == float.MaxValue && minDepth != float.MaxValue)
            {
                BackgroundAura.UpdateAura();

                if (BackgroundAura.AuraStillVisible)
                {
                    var config = ModContent.GetInstance<StariteConfig>();
                    if (GlimmerEvent.IsGlimmerEventCurrentlyActive())
                    {
                        if (CanSpawnBGStarites())
                            SpawnBGStarites(rand);
                        BackgroundAura.Brightness *= 1f - (GlimmerEvent.tileX - (Main.screenPosition.X + Main.screenWidth) / 16f).Abs() / GlimmerEvent.MaxDistance;
                    }
                    if (BackgroundAura.AuraStillVisible)
                        BackgroundAura.RenderAura();
                }
                else
                {
                    _starites = null;
                }

                if (FallingStars.stars != null && FallingStars.stars.Count != 0)
                {
                    AQUtils.BGTop = (int)(-Main.screenPosition.Y / (Main.worldSurface * 16.0 - 600.0) * 200.0);
                    //Main.NewText(AQUtils.BGTop, Main.DiscoColor);
                    foreach (var s in FallingStars.stars)
                    {
                        s.Render();
                    }
                }

                if (_starites != null)
                {
                    if (Main.BackgroundEnabled)
                    {
                        for (int i = 0; i < 120; i++)
                        {
                            _starites[i].Draw(rand);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 150; i++)
                        {
                            _starites[i].Draw(rand);
                        }
                    }
                }
            }
            else if (Main.BackgroundEnabled)
            {
                if (_starites != null)
                {
                    const float layerDepth = 7f;
                    const float layerDepth2 = 4f;
                    const float layerDepth3 = 2f;
                    if (maxDepth <= layerDepth && minDepth >= layerDepth2)
                    {
                        for (int i = 120; i < 135; i++)
                        {
                            _starites[i].Draw(rand);
                        }
                    }
                    if (maxDepth <= layerDepth2 && minDepth >= layerDepth3)
                    {
                        for (int i = 135; i < 145; i++)
                        {
                            _starites[i].Draw(rand);
                        }
                    }
                    if (minDepth <= layerDepth3)
                    {
                        for (int i = 145; i < 150; i++)
                        {
                            _starites[i].Draw(rand);
                        }
                    }
                }
            }
        }

        public override bool IsActive()
        {
            return (_active || _starites != null || BackgroundAura.AuraStillVisible || FallingStars.stars != null) && !AQMod.Loading;
        }

        public override float GetCloudAlpha()
        {
            if (GlimmerEvent.IsGlimmerEventCurrentlyActive() || OmegaStariteScenes.OmegaStariteIndexCache != -1)
            {
                _cloudAlpha = MathHelper.Lerp(_cloudAlpha, 0.25f, 0.01f);
            }
            else
            {
                _cloudAlpha = MathHelper.Lerp(_cloudAlpha, 1f, 0.01f);
            }
            return _cloudAlpha;
        }

        public override void Reset()
        {
        }

        public override void OnLoad() { }

        public override void Activate(Vector2 position, params object[] args)
        {
            _active = true;
            BackgroundAura.Activate(0.01f);
        }

        public override void Deactivate(params object[] args)
        {
            _active = false;
            BackgroundAura.Deactivate(0.01f);
        }
    }
}