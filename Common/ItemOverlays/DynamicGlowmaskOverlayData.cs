using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace AQMod.Common.ItemOverlays
{
    public class DynamicGlowmaskOverlayData : GlowmaskOverlayData
    {
        public readonly Func<Color> GetColor;

        internal DynamicGlowmaskOverlayData(string path) : base(path)
        {
            throw new Exception("Incorrectly created a dynamic glowmask overlay");
        }

        internal DynamicGlowmaskOverlayData(string path, Func<Color> getColor) : base(path, getColor())
        {
            GetColor = getColor;
        }

        internal DynamicGlowmaskOverlayData(string path, Func<Color> getColor, int shader) : base(path, getColor(), shader)
        {
            GetColor = getColor;
        }

        public override bool PreDraw(bool drawingOnPlayer, Item item)
        {
            drawColor = GetColor();
            return true;
        }
    }
}
