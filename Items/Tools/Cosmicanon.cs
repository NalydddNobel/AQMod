using AQMod.Content;
using AQMod.Localization;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class Cosmicanon : ModItem, IUpdatePiggybank
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.accessory = true;
            item.rare = AQItem.Rarities.OmegaStariteRare;
            item.value = Item.buyPrice(gold: 60);
        }

        private void Update(Player player)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.cosmicanon = true;
            if (!aQPlayer.IgnoreIgnoreMoons)
                aQPlayer.ignoreMoons = true;
        }

        public override void UpdateInventory(Player player)
        {
            Update(player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Update(player);
        }

        void IUpdatePiggybank.UpdatePiggyBank(Player player, int i)
        {
            Update(player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.mod == "Terraria" && t.Name == "Tooltip0")
                {
                    t.text += "\n" + Language.GetTextValue("Mods.AQMod.Tooltips.ToggleKeyBind", AQText.KeybindText(AQMod.Keybinds.CosmicanonToggle));
                }
                if (t.mod == "Terraria" && t.Name == "Tooltip1")
                {
                    t.text += "\n" + Language.GetTextValue("Mods.AQMod.ItemTooltipExtra.Cosmicanon.BloodMoonsPrevented", CosmicanonCounts.BloodMoonsPrevented);
                    t.text += "\n" + Language.GetTextValue("Mods.AQMod.ItemTooltipExtra.Cosmicanon.GlimmersPrevented", CosmicanonCounts.GlimmersPrevented);
                    t.text += "\n" + Language.GetTextValue("Mods.AQMod.ItemTooltipExtra.Cosmicanon.EclipsesPrevented", CosmicanonCounts.EclipsesPrevented);
                }
            }
        }
    }
}