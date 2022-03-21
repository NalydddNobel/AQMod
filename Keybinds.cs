using Terraria.ModLoader;

namespace AQMod
{
    public sealed class Keybinds
    {
        public static ModHotKey ArmorSetBonus { get; private set; }

        internal static void Load()
        {
            var mod = AQMod.Instance;
            ArmorSetBonus = mod.RegisterHotKey("Armor Set Bonus", "V");
        }

        internal static void Unload()
        {
            ArmorSetBonus = null;
        }
    }
}