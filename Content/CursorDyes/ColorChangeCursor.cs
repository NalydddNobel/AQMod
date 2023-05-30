using Microsoft.Xna.Framework;
using System;

namespace Aequus.Content.CursorDyes {
    public class ColorChangeCursor : ICursorDye
    {
        public int Type { get; set; }
        public Func<Color> GetColor;

        public ColorChangeCursor(Color color)
        {
            GetColor = () => color;
        }
        public ColorChangeCursor(Func<Color> getColor)
        {
            GetColor = getColor;
        }

        Color? ICursorDye.GetCursorColor()
        {
            return GetColor();
        }
    }
}