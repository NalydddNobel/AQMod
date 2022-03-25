using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public sealed class Keybinds
    {
        public ModHotKey ArmorSetBonus { get; private set; }

        public Keybinds(AQMod aQMod)
        {
            ArmorSetBonus = aQMod.RegisterHotKey("Armor Set Bonus", "V");
        }

        public static string GetText(ModHotKey key)
        {
            foreach (var k in key.GetAssignedKeys())
            {
                return Language.GetTextValue("Mods.AQMod.Common.KeyBound", k);
            }
            return Language.GetTextValue("Mods.AQMod.Common.KeyUnbound");
        }
    }
}