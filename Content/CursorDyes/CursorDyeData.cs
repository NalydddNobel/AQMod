using AQMod.Content.CursorDyes.Components;
using Microsoft.Xna.Framework;

namespace AQMod.Content.CursorDyes
{
    public struct CursorDyeData
    {
        public readonly Vector2? CursorDyeThickCursor;
        private readonly ICursorDyeComponent[] _components;

        public CursorDyeData(params ICursorDyeComponent[] components)
        {
            CursorDyeThickCursor = new Vector2(2f);
            _components = components;
        }
        public CursorDyeData(Vector2 thickCursorBonus, params ICursorDyeComponent[] components)
        {
            CursorDyeThickCursor = thickCursorBonus;
            _components = components;
        }

        public void UpdateComponents()
        {
            if (_components != null)
            {
                foreach (ICursorDyeComponent c in _components)
                {
                    c.OnUpdateUI();
                }
            }
        }

        public bool PreRender(bool cursorOverride, bool smart = false)
        {
            if (_components != null)
            {
                foreach (ICursorDyeComponent co in _components)
                {
                    if (co is CursorDyeTextureChangeComponent c && !c.PreRender(cursorOverride, smart))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void PostRender(bool cursorOverride, bool smart = false)
        {
            if (_components != null)
            {
                foreach (ICursorDyeComponent co in _components)
                {
                    if (co is CursorDyeTextureChangeComponent c)
                    {
                        c.PostRender(cursorOverride, smart);
                    }
                }
            }
        }
    }
}