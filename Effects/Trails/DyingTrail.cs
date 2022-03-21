using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Effects.Trails
{
    public sealed class DyingTrail : Trail
    {
        private readonly Texture2D _texture;
        private readonly string _pass;
        private readonly List<Vector2> _positions;
        private readonly Func<float, Vector2> _getSize;
        private readonly Func<float, Color> _getColor;
        private readonly float _progressAdd;
        private readonly float _coordsMult;
        private readonly int _extraUpdates;

        public DyingTrail(Texture2D texture, string pass, List<Vector2> positions, Func<float, Vector2> getSize, Func<float, Color> getColor, float progressAdd = 0f, float coordsMult = 1f, int extraUpdates = 0)
        {
            _texture = texture;
            _pass = pass;
            _positions = positions;
            _getSize = getSize;
            _getColor = getColor;
            _progressAdd = progressAdd;
            _coordsMult = coordsMult;
            _extraUpdates = extraUpdates;
        }

        public override bool Update()
        {
            for (int i = 0; i < _extraUpdates + 1; i++)
            {
                if (_positions.Count <= 3)
                {
                    return false;
                }
                _positions.RemoveAt(_positions.Count - 1);
            }
            return true;
        }

        public override void Render()
        {
            PrimitivesRenderer.FullDraw(_texture, _pass, _positions.AsAddAll(-Main.screenPosition).ToArray(),
                _getSize, _getColor, _progressAdd, _coordsMult);
        }
    }
}