﻿namespace AQMod.Content.CursorDyes
{
    public class CursorDyeLoader : ContentLoader<CursorDye>
    {
        public static class ID
        {
            public const int None = -1;
            public const int Health = 0;
            public const int Mana = 1;
            public const int Sword = 2;
            public const int Demon = 3;
        }

        public override void Setup(bool setupStatics = false)
        {
            base.Setup(setupStatics);

            var mod = AQMod.Instance;
            AddContent(new CursorDyeHealth(mod, "Health"));
            AddContent(new CursorDyeMana(mod, "Mana"));
            AddContent(new CursorDyeSword(mod, "Sword"));
            AddContent(new CursorDyeDemon(mod, "Demon"));
        }
    }
}