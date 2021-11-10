using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets.Graphics.DrawTypes
{
    public class ArrowDraw : IDrawType
    {
        private readonly Color _color;
        private List<Vector2> _trail;
        private List<float> _trailRots;

        public ArrowDraw(Color color, Vector2[] trail, float headRot = 0f)
        {
            _color = color;
            _trail = new List<Vector2>();
            _trailRots = new List<float>();
            for (int i = 0; i < trail.Length; i++)
            {
                _trail.Add(trail[i]);
                if (i == 0)
                    _trailRots.Add(headRot);
                else
                    _trailRots.Add((trail[i - 1] - trail[i]).ToRotation());
            }
        }

        public ArrowDraw(Color color, List<Vector2> trail, float headRot = 0f)
        {
            _color = color;
            _trail = new List<Vector2>();
            _trailRots = new List<float>();
            for (int i = 0; i < trail.Count; i++)
            {
                _trail.Add(trail[i]);
                _trailRots.Add(0f);
            }
        }

        public void RunDraw()
        {
            var texture = ModContent.GetTexture("AQMod/Assets/Textures/ArrowLine");
            var origin = texture.Size() / 2f;
            Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/Textures/Arrow"), _trail[0], null, _color, _trailRots[0] + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
            for (int i = 1; i < _trail.Count; i++)
            {
                Main.spriteBatch.Draw(texture, _trail[i], null, _color, _trailRots[i] + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}