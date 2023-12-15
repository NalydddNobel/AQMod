using Aequus.Core.Graphics.Animations;
using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;

namespace Aequus.Content.Wires;

public class ConductiveAnimation : ITileAnimation {
    public int timeActive;
    public float intensity;
    public float electricAnimation;

    public static float AnimationTime { get; set; } = 16f;

    public bool Update(int x, int y) {
        electricAnimation = MathF.Sin(timeActive / AnimationTime * MathHelper.Pi);
        if (electricAnimation > 0.1f && Main.rand.NextBool(7)) {
            var d = Dust.NewDustDirect(new Vector2(x * 16f, y * 16f), 16, 16, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 1.8f));
            d.rotation = 0f;
            //d.fadeIn = d.scale + 0.4f;
            d.noGravity = true;
        }
        return timeActive++ < AnimationTime;
    }
}