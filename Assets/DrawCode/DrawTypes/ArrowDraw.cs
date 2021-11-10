using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets.DrawCode.DrawTypes
{
    public class ArrowDraw : IDrawType
    {
        private readonly Color _color;
        private List<Vector2> _trail;
        private List<float> _trailRots;

        public ArrowDraw(Color color, Vector2[] trail)
        {
            _color = color;
            _trail = new List<Vector2>();
            _trailRots = new List<float>();
            for (int i = 0; i < trail.Length; i++)
            {
                _trail.Add(trail[i]);
                _trailRots.Add(0f);
            }
        }

        public ArrowDraw(Color color, List<Vector2> trail)
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
            for (int i = 0; i < _trail.Count; i++)
            {
                Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/Textures/ArrowLine"), _trail[i], null, _color, _trailRots[i], new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0f);
            }
        }
    }
}