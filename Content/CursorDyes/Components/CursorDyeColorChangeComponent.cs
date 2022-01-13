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
            GetColor = getColor;
        }

        void ICursorDyeComponent.OnUpdateUI()
        {
            CursorDyeManager.Hooks.NewCursorColor = GetColor();
        }
    }
}