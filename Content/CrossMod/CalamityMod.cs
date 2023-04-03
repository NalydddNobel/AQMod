using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Aequus.Content.CrossMod
{
    internal class CalamityMod : ModSupport<CalamityMod>
    {
        public static bool Revengeance => DifficultyActive("revengeance");
        public static bool Death => DifficultyActive("death");
        public static bool BossRush => DifficultyActive("bossrush");

        public static bool DifficultyActive(string difficultyName) {
            if (Instance == null) {
                return false;
            }

            return TryCall(out bool value, "DifficultyActive", difficultyName) ? value : false;
        }

        public static void ModifyTooltips_RevengenceTooltip(Item item, List<TooltipLine> tooltips) {

            if (Instance == null || !item.master || item.ModItem?.Mod is not Aequus) {
                return;
            }

            foreach (var t in tooltips) {
                if (t.Name == "Master") {
                    t.Text += Language.GetTextValue("Mods.Aequus.Common.MoR");
                }
            }
        }
    }
}