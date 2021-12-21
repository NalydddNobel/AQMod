using AQMod.Common.Graphics.SceneLayers;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Effects.WorldEffects
{
    public class UltimateSwordEffect : WorldVisualEffect
    {
        private float _time;
        private readonly float _maxWidth;
        private readonly float _maxHeight;
        private byte _existence;
        private readonly byte _lifespan;
        private readonly byte _reps;
        private Color _dustColor;
        private readonly UnifiedRandom _rand;

        public UltimateSwordEffect(UnifiedRandom rand) : base(GlimmerEvent.tileX * 16, GlimmerEvent.tileY * 16)
        {
            _rand = rand;
            _time = rand.NextFloat(0f, MathHelper.PiOver2 * 3f);
            _maxWidth = rand.NextFloat(10f, 35f);
            _maxHeight = rand.NextFloat(55f, 120f);
            _reps = (byte)(rand.Next(4) + 1);
            _lifespan = (byte)(rand.Next(50, 150) + 1);
            _existence = (byte)rand.Next(_lifespan / 3);
            int colorType = rand.Next(9);
            switch (colorType)
            {
                default:
                {
                    _dustColor = new Color(255, 255, 100, 0);
                }
                break;

                case 1:
                {
                    _dustColor = new Color(140, 50, 255, 0);
                }
                break;

                case 2:
                {
                    _dustColor = new Color(255, 255, 255, 0);
                }
                break;

                case 3:
                {
                    _dustColor = new Color(255, 100, 255, 0);
                }
                break;

                case 4:
                {
                    _dustColor = new Color(255, 160, 255, 0);
                }
                break;

                case 5:
                {
                    _dustColor = new Color(160, 255, 180, 0);
                }
                break;

                case 6:
                {
                    _dustColor = new Color(40, 255, 150, 0);
                }
                break;

                case 7:
                {
                    _dustColor = new Color(180, 255, 50, 0);
                }
                break;

                case 8:
                {
                    _dustColor = new Color(255, 100, 180, 0);
                }
                break;
            }
        }

        public UltimateSwordEffect(float timeOffset, float maxWidth, float maxHeight, int lifespan, int reps, Color dustColor, int seed = -1) : base(GlimmerEvent.tileX * 16, GlimmerEvent.tileY * 16)
        {
            if (seed == -1)
                _rand = new UnifiedRandom();
            else
                _rand = new UnifiedRandom(seed);
            _time = timeOffset;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
            _existence = 0;
            _reps = (byte)reps;
            _lifespan = (byte)lifespan;
            _dustColor = dustColor;
        }

        public override bool Update()
        {
            for (int i = 0; i < _reps; i++)
            {
                _existence++;
                float progress = 1f - (_existence / (float)_lifespan);
                if (_time < MathHelper.TwoPi)
                {
                    _time += _existence * 0.0008f;
                }
                else
                {
                    _time += _existence * (0.0005f * (1f - progress));
                }
                var s = UltimateSwordWorldOverlay.swordPos();
                int x = (int)s.X + (int)(Math.Sin(_time) * _maxWidth);
                int y = (int)s.Y - (int)((1f - progress) * _maxHeight);
                int d = Dust.NewDust(new Vector2(x, y), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, _dustColor);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.1f;
                if (AQConfigClient.c_EffectQuality >= 1f)
                {
                    float p = 1f - (progress * 0.5f);
                    Main.dust[d].scale *= p * p;
                    if (_rand.NextBool(20))
                    {
                        Main.dust[d].scale *= _rand.NextFloat(1.1f, 1.5f);
                    }
                    Main.dust[d].color *= p;
                    if (_rand.NextBool(15))
                    {
                        d = Dust.NewDust(new Vector2(x, y), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, _dustColor);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 0.8f;
                    }
                }
                if (_existence >= _lifespan)
                {
                    return false;
                }
            }
            return true;
        }
    }
}