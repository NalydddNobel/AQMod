using Aequus.Core.Debugging;
using System;
using System.Collections.Generic;

namespace Aequus.Core.Graphics;

[Autoload(Side = ModSide.Client)]
public class DrawLayers : ILoad {
    /// <summary>Invoked before NPCs behind tiles are drawn.</summary>
    public event Action<SpriteBatch> WorldBehindTiles;
    /// <summary>Invoked after Dusts have been drawn.</summary>
    public event Action<SpriteBatch> PostDrawDust;
    /// <summary>Invoked after Liquids and Inferno Rings have been drawn, but before Wire Overlays are drawn.</summary>
    public DrawLayer PostDrawLiquids;

    public static DrawLayers Instance { get; private set; }

    public void Load(Mod mod) {
        Instance = this;
        On_Main.DrawNPCs += On_Main_DrawNPCs;
        On_Main.DrawDust += On_Main_DrawDust;
        On_Main.DrawInfernoRings += On_Main_DrawInfernoRings;
    }

    private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles) {
        if (behindTiles) {
            Instance.WorldBehindTiles?.Invoke(Main.spriteBatch);
        }

        orig(self, behindTiles);
    }

    public void Unload() {
    }

    private static void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main main) {
        orig(main);
        Instance.PostDrawDust?.Invoke(Main.spriteBatch);
    }


    private static void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self) {
        orig(self);
        DiagnosticsMenu.StartStopwatch();

        Instance.PostDrawLiquids?.Draw(Main.spriteBatch);

        DiagnosticsMenu.EndStopwatch(DiagnosticsMenu.TimerType.PostDrawLiquids);
    }

    public class DrawLayer {
        private Action<SpriteBatch> _action;
        private readonly List<IDrawLayer> _drawInstances = new(2);
        private readonly List<int> _npcsIndices = new(2);
        private readonly List<int> _projIndices = new(2);

        internal void Draw(SpriteBatch spriteBatch) {
            if (_drawInstances.Count > 0) {
                for (int i = 0; i < _drawInstances.Count; i++) {
                    _drawInstances[i].DrawOntoLayer(spriteBatch, this);
                }
                _drawInstances.Clear();
            }

            if (_npcsIndices.Count > 0) {
                for (int i = 0; i < _npcsIndices.Count; i++) {
                    NPC npc = Main.npc[_npcsIndices[i]];
                    if (!npc.active || npc.ModNPC is not IDrawLayer drawLayer) {
                        continue;
                    }

                    npc.position += npc.netOffset;
                    drawLayer.DrawOntoLayer(spriteBatch, this);
                    npc.position -= npc.netOffset;
                }
                _npcsIndices.Clear();
            }

            if (_projIndices.Count > 0) {
                for (int i = 0; i < _projIndices.Count; i++) {
                    Projectile proj = Main.projectile[_projIndices[i]];
                    if (!proj.active || proj.ModProjectile is not IDrawLayer drawLayer) {
                        continue;
                    }

                    drawLayer.DrawOntoLayer(spriteBatch, this);
                }
                _projIndices.Clear();
            }

            _action?.Invoke(spriteBatch);
        }

        public static DrawLayer operator +(DrawLayer a, IDrawLayer b) {
            (a ??= new())._drawInstances.Add(b);
            return a;
        }
        public static DrawLayer operator +(DrawLayer a, Projectile b) {
            (a ??= new())._projIndices.Add(b.whoAmI);
            return a;
        }
        public static DrawLayer operator +(DrawLayer a, NPC b) {
            (a ??= new())._npcsIndices.Add(b.whoAmI);
            return a;
        }
        public static DrawLayer operator +(DrawLayer a, Action<SpriteBatch> b) {
            (a ??= new())._action += b;
            return a;
        }
        public static DrawLayer operator -(DrawLayer a, Action<SpriteBatch> b) {
            (a ??= new())._action -= b;
            return a;
        }
    }

    public interface IDrawLayer {
        void DrawOntoLayer(SpriteBatch spriteBatch, DrawLayer layer);
    }
}
