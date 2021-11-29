using AQMod.Assets.Graphics;
using AQMod.Common.Graphics.SceneLayers;
using System.Collections.Generic;

namespace AQMod.Common.Graphics.CustomPickupText
{
    public sealed class CustomPickupTextLayer : SceneLayer
    {
        private static List<IDrawObject> _text;

        public override string Name => "CustomPickupText";
        public override SceneLayering Layering => SceneLayering.PostDrawPlayers;

        protected override void OnRegister(LayerKey key)
        {
            _text = new List<IDrawObject>();
        }

        public static void NewText(IDrawObject draw)
        {
            _text.Add(draw);
        }

        protected override void Draw()
        {
            foreach (var d in _text)
            {
                d.RunDraw();
            }
        }

        public override void Update()
        {
            for (int i = 0; i < _text.Count; i++)
            {
                IDrawObject d = _text[i];
                if (d.Update())
                {
                    _text.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}