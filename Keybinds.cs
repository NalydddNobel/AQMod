using Terraria.ModLoader;

namespace AQMod
{
    public sealed class Keybinds
    {
        public static ModHotKey ArmorSetBonus { get; private set; }
        public static ModHotKey CosmicanonToggle { get; private set; }

        internal static void Load()
        {
            var mod = AQMod.GetInstance();
            ArmorSetBonus = mod.RegisterHotKey("Armor Set Bonus", "V");
            CosmicanonToggle = mod.RegisterHotKey("Cosmicanon Toggle", "P");
        }

        internal static void Unload()
        {
            CosmicanonToggle = null;
            ArmorSetBonus = null;
        }
    }
}