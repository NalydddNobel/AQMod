using AQMod.Assets;
using AQMod.Content.WorldEvents.GlimmerEvent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Common.Skies
{
    public class GlimmerEventSky : CustomSky
    {
        public const string Name = "AQMod:GlimmerEventSky";

        public static float _glimmerLight;
        private float _cloudAlpha;
        private static bool _active;

        private static UnifiedRandom _random;
        public static float _lonelyStariteX;
        public static float _lonelyStariteY;
        public static float _lonelyStariteTimeLeft;
        public static BGStarite _lonelyStarite;
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

        internal static void ModLoad()
        {
            _random = new UnifiedRandom();
            BGStarite._texture = ModContent.GetTexture("AQMod/Assets/Backgrounds/Entities/Starite");
        }

        internal static void Unload()
        {
            _random = null;
            _starites = null;
            _lonelyStarite = null;
        }

        public static bool CanSpawnBGStarites()
        {
            return _starites == null && AQMod.ShowBackgroundStarites;
        }

        public static void InitNight()
        {
            if (!AQMod.CosmicEvent.IsActive)
            {
                if (CanSpawnBGStarites() && AQMod.VariableLuck(AQMod.CosmicEvent.spawnChance, _random))
                {
                    _lonelyStarite = new BGStarite
                    {
                        x = (Main.screenWidth / 2f) + _random.Next(-20, 20),
                        y = _random.Next(-40, -12),
                        size = _random.Next(4)
                    };
                    _lonelyStariteTimeLeft = _random.Next(1000, (int)Main.nightLength);
                }
            }
            else
            {
                SpawnBGStarites(_random);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_starites != null)
            {
                float midX = Main.screenWidth / 2f + (float)Math.Sin(Main.GlobalTime) * 125f;
                float midY = Main.screenHeight / 2f + (float)Math.Cos(Main.GlobalTime) * 125f;
                for (int i = 0; i < 120; i++)
                {
                    _starites[i].Update(midX, midY, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.X / Main.screenPosition.Y + i * 0.0125f));
                }
                for (int i = 120; i < 135; i++)
                {
                    _starites[i].Update(midX, midY - Main.screenHeight / 2f, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.Y / Main.screenPosition.X + i * 0.12f));
                }
                for (int i = 135; i < 145; i++)
                {
                    _starites[i].Update(midX, midY - Main.screenHeight / 3f, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.Y * Main.screenPosition.X + i * 0.12f));
                }
                for (int i = 145; i < 150; i++)
                {
                    _starites[i].Update(midX, midY - Main.screenHeight / 5f, (float)Math.Sin(Main.GlobalTime * 0.25f + Main.screenPosition.Y * Main.screenPosition.X + i));
                }
            }
            else if (_lonelyStarite != null)
            {
                if (_lonelyStariteTimeLeft == 0)
                {
                    _lonelyStarite.x += _lonelyStarite.velocityX;
                    _lonelyStarite.y += _lonelyStarite.velocityY;
                    if (_lonelyStarite.x <= -30f || _lonelyStarite.x >= Main.screenWidth + 30 || _lonelyStarite.y <= -30f || _lonelyStarite.y > Main.screenHeight + 30)
                        _lonelyStarite = null;
                }
                else
                {
                    _lonelyStariteTimeLeft--;
                    var plr = Main.LocalPlayer;
                    _lonelyStariteX = Main.screenWidth / 2f + (float)Math.Sin(Main.GlobalTime * 10f) * 175f + plr.velocity.X * 2f;
                    _lonelyStariteY = Main.screenHeight / 2f + (float)Math.Cos(Main.GlobalTime * 10f) * 175f + plr.velocity.Y * 2f;
                    _lonelyStarite.Update(_lonelyStariteX, _lonelyStariteY, (float)Math.Sin(Main.GlobalTime) * 0.1f);
                }
            }
            if (Main.dayTime)
                _lonelyStarite = null;
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

        private void DrawSky()
        {
            if (OmegaStariteScenes.OmegaStariteIndexCache != -1)
            {
                int width = Main.screenWidth + 40;
                int height = Main.screenHeight + 40;
                var texture = AQTextures.Lights[LightTex.Spotlight66x66];
                float scaleX = width / texture.Width * 1.75f;
                float scaleY = height / texture.Height;
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                Color color;
                float y = Main.screenHeight / 2f + (325f - Main.screenPosition.Y / 16f);
                if (AQMod.CosmicEvent.StariteDisco)
                {
                    color = new Color((int)(Main.DiscoR * 0.9f + Main.DiscoG * 0.05f + Main.DiscoB * 0.05f), (int)(Main.DiscoG * 0.9f + Main.DiscoR * 0.05f + Main.DiscoB * 0.05f), (int)(Main.DiscoB * 0.9f + Main.DiscoR * 0.05f + Main.DiscoG * 0.05f), 255);
                }
                else
                {
                    color = AQMod.StariteAuraColor;
                    color.A = 255;
                    color *= AQMod.StariteBGMult;
                }
                color *= (1f + (ModContent.GetInstance<AQConfigClient>().EffectIntensity - 1f) * 0.24f) * (1f - _glimmerLight);
                float x = Main.screenWidth / 2f;
                Main.spriteBatch.Draw(texture, new Vector2(x, y), null, color, 0f, frame.Size() / 2f, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
                if (_glimmerLight > 0f)
                {
                    _glimmerLight -= 0.0125f;
                    if (_glimmerLight < 0f)
                        _glimmerLight = 0f;
                }
            }
            else if (AQMod.CosmicEvent.IsActive)
            {
                if (CanSpawnBGStarites())
                    SpawnBGStarites(_random);
                float tileDistance = (AQMod.CosmicEvent.tileX - (Main.screenPosition.X + Main.screenWidth) / 16f).Abs();
                float intensity = (1f - tileDistance / GlimmerEvent.MaxDistance) * ModContent.GetInstance<AQConfigClient>().EffectIntensity;
                if (Main.time < 200.0)
                    intensity *= (float)Main.time / 200f;
                if (intensity > 0f)
                {
                    int width = Main.screenWidth + 40;
                    int height = Main.screenHeight + 40;
                    var texture = AQTextures.Lights[LightTex.Spotlight66x66];
                    float scaleX = width / texture.Width * 1.75f;
                    float scaleY = height / texture.Height;
                    var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    Color color;
                    float y = Main.screenHeight / 2f + (325f - Main.screenPosition.Y / 16f);
                    if (AQMod.CosmicEvent.StariteDisco)
                    {
                        color = new Color((int)(Main.DiscoR * 0.9f + Main.DiscoG * 0.05f + Main.DiscoB * 0.05f), (int)(Main.DiscoG * 0.9f + Main.DiscoR * 0.05f + Main.DiscoB * 0.05f), (int)(Main.DiscoB * 0.9f + Main.DiscoR * 0.05f + Main.DiscoG * 0.05f), 255);
                    }
                    else
                    {
                        color = AQMod.StariteAuraColor;
                        color.A = 255;
                        color *= AQMod.StariteBGMult;
                    }
                    color *= intensity;
                    float x = Main.screenWidth / 2f;
                    Main.spriteBatch.Draw(texture, new Vector2(x, y), null, color, 0f, frame.Size() / 2f, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
                }
            }
            else
            {
                _starites = null;
                if (_lonelyStarite != null && (_lonelyStarite.size == 0 || !Main.BackgroundEnabled))
                    _lonelyStarite.Draw(_random);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth == float.MaxValue && minDepth != float.MaxValue)
            {
                DrawSky();
                if (_starites != null)
                {
                    if (Main.BackgroundEnabled)
                    {
                        for (int i = 0; i < 120; i++)
                        {
                            _starites[i].Draw(_random);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 150; i++)
                        {
                            _starites[i].Draw(_random);
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
                            _starites[i].Draw(_random);
                        }
                    }
                    if (maxDepth <= layerDepth2 && minDepth >= layerDepth3)
                    {
                        for (int i = 135; i < 145; i++)
                        {
                            _starites[i].Draw(_random);
                        }
                    }
                    if (minDepth <= layerDepth3)
                    {
                        for (int i = 145; i < 150; i++)
                        {
                            _starites[i].Draw(_random);
                        }
                    }
                }
                else if (_lonelyStarite != null && _lonelyStarite.size > 0)
                {
                    const float layerDepth = 7f;
                    const float layerDepth2 = 4f;
                    const float layerDepth3 = 2f;
                    switch (_lonelyStarite.size)
                    {
                        case 1:
                        if (maxDepth <= layerDepth && minDepth >= layerDepth2)
                            _lonelyStarite.Draw(_random);
                        break;

                        case 2:
                        if (maxDepth <= layerDepth2 && minDepth >= layerDepth3)
                            _lonelyStarite.Draw(_random);
                        break;

                        case 3:
                        if (minDepth <= layerDepth3)
                            _lonelyStarite.Draw(_random);
                        break;
                    }
                }
            }
        }

        public override bool IsActive()
        {
            return _active;
        }

        public override float GetCloudAlpha()
        {
            if (AQMod.CosmicEvent.IsActive || OmegaStariteScenes.OmegaStariteIndexCache != -1)
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
            ModLoad();
        }

        public override void OnLoad() { }

        public override void Activate(Vector2 position, params object[] args)
        {
            _active = true;
        }

        public override void Deactivate(params object[] args)
        {
            _active = false;
        }
    }
}