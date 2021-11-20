using System.Collections.Generic;

namespace AQMod.Assets.Graphics.SceneLayers
{
    public sealed class CustomPickupTextLayer : SceneLayer
    {
        public const string Name = "HotAndColdCurrent";
        public const SceneLayering Layer = SceneLayering.PostDrawPlayers;

        private List<IDrawObject> _text;

        public void AddDraw(IDrawObject draw)
        {
            _text.Add(draw);
        }

        public static void NewText(IDrawObject text)
        {
            ((CustomPickupTextLayer)AQMod.WorldLayers.GetLayer(Layer, Name)).AddDraw(text);
        }

        protected override void onLoad()
        {
            _text = new List<IDrawObject>();
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