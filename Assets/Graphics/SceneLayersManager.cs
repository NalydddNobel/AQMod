using System.Collections.Generic;
using Terraria;

namespace AQMod.Assets.Graphics
{
    public sealed class SceneLayersManager
    {
        private Dictionary<string, SceneLayer>[] _layers;

        public void Register(string name, SceneLayer layer, SceneLayering layering)
        {
            if (AQMod.Loading)
            {
                _layers[(byte)layering].Add(name, layer);
                _layers[(byte)layering][name].OnLoad();
            }
        }

        public SceneLayer GetLayer(SceneLayering layering, string name)
        {
            return _layers[(byte)layering][name];
        }

        private SceneLayer[] GetEntireLayer(SceneLayering layering)
        {
            var l = _layers[(byte)layering];
            var layers = new SceneLayer[l.Count];
            int i = 0;
            foreach (var layer in l)
            {
                layers[i] = layer.Value;
                i++;
            }
            return layers;
        }

        /// <summary>
        /// Draws an entire layer
        /// </summary>
        /// <param name="layering"></param>
        internal void DrawLayer(SceneLayering layering)
        {
            if (Main.gameMenu)
            {
                return;
            }
            foreach (var layer in _layers[(byte)layering])
            {
                layer.Value.DrawLayer();
            }
        }

        public void UpdateLayers()
        {
            foreach (var d in _layers)
            {
                foreach (var l in d)
                {
                    l.Value.Update();
                }
            }
        }

        public void Initialize()
        {
            foreach (var d in _layers)
            {
                foreach (var l in d)
                {
                    l.Value.Initialize();
                }
            }
        }

        internal void Setup(bool loadHooks = true)
        {
            if (loadHooks)
            {
                On.Terraria.Main.DrawNPCs += Main_DrawNPCs;
            }
            _layers = new Dictionary<string, SceneLayer>[(byte)SceneLayering.Count];
            for (byte i = 0; i < (byte)SceneLayering.Count; i++)
            {
                _layers[i] = new Dictionary<string, SceneLayer>();
            }
        }

        private static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (behindTiles)
            {
                AQMod.WorldLayers.DrawLayer(SceneLayering.BehindTiles_BehindNPCs);
            }
            else
            {
                AQMod.WorldLayers.DrawLayer(SceneLayering.BehindNPCs);
            }
            orig(self, behindTiles);
            if (behindTiles)
            {
                AQMod.WorldLayers.DrawLayer(SceneLayering.BehindTiles_InfrontNPCs);
            }
            else
            {
                AQMod.WorldLayers.DrawLayer(SceneLayering.InfrontNPCs);
            }
        }

        internal void Unload()
        {
            if (_layers != null)
            {
                string failedLayer = "";
                byte failedStep = 0;
                try
                {
                    failedStep = 0;
                    foreach (var dictionary in _layers)
                    {
                        failedStep = 1;
                        foreach (var layer in dictionary)
                        {
                            failedStep = 2;
                            failedLayer = layer.Key;
                            failedStep = 3;
                            layer.Value.Unload();
                        }
                        failedStep = 4;
                    }
                    failedStep = 5;
                }
                catch
                {
                    if (AQMod.Instance != null && AQMod.Instance.Logger != null)
                    {
                        var l = AQMod.Instance.Logger;
                        l.Error("Couldn't fully unload scene layers");
                        l.Error("Failed Step: " + failedStep);
                        l.Error("Failed Layer: " + failedLayer);
                    }
                }
                _layers = null;
            }
        }
    }
}
