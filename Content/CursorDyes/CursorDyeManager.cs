using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public class CursorDyeManager : ContentLoader<CursorDye>
    {
        public static class ID
        {
            public const int None = -1;
            public const int Health = 0;
            public const int Mana = 1;
            public const int Sword = 2;
            public const int Demon = 3;
        }

        public static CursorDyeManager Instance { get => ModContent.GetInstance<CursorDyeManager>(); }

        public override void Load(AQMod mod)
        {
            InitializeContent(new CursorDyeHealth(mod, "Health"));
            InitializeContent(new CursorDyeMana(mod, "Mana"));
            InitializeContent(new CursorDyeSword(mod, "Sword"));
            InitializeContent(new CursorDyeDemon(mod, "Demon"));
        }
    }
}