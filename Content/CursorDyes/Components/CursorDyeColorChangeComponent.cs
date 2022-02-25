using AQMod.Items.Dyes.Cursor;
using Microsoft.Xna.Framework;
using System;

namespace AQMod.Content.CursorDyes.Components
{
    public class CursorDyeColorChangeComponent : ICursorDyeComponent
    {
        private Func<Color> GetColor;

        public CursorDyeColorChangeComponent(Color color)
        {
            GetColor = () => color;
        }
        public CursorDyeColorChangeComponent(Func<Color> getColor)
        {
            CursorDyeManager.OverridingColor = true;
            GetColor = getColor;
        }

        void ICursorDyeComponent.OnUpdateUI()
        {
            CursorDyeManager.OverridingColor = true;
            CursorDyeManager.Hooks.NewCursorColor = GetColor();
        }
    }
}