using System;
using System.Collections.Generic;

namespace Aequus.Common.Drawing;

public class DrawLayer {
    private Action<SpriteBatch>? _action;
    private readonly List<IDrawOntoLayer> _drawInstances = new(2);
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
                if (!npc.active || npc.ModNPC is not IDrawOntoLayer drawLayer) {
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
                if (!proj.active || proj.ModProjectile is not IDrawOntoLayer drawLayer) {
                    continue;
                }

                drawLayer.DrawOntoLayer(spriteBatch, this);
            }
            _projIndices.Clear();
        }

        _action?.Invoke(spriteBatch);
    }

    public static DrawLayer operator +(DrawLayer a, IDrawOntoLayer b) {
        a._drawInstances.Add(b);
        return a;
    }
    public static DrawLayer operator +(DrawLayer a, Projectile b) {
        a._projIndices.Add(b.whoAmI);
        return a;
    }
    public static DrawLayer operator +(DrawLayer a, NPC b) {
        a._npcsIndices.Add(b.whoAmI);
        return a;
    }
    public static DrawLayer operator +(DrawLayer a, Action<SpriteBatch> b) {
        a._action += b;
        return a;
    }
    public static DrawLayer operator -(DrawLayer a, Action<SpriteBatch> b) {
        a._action -= b;
        return a;
    }
}
