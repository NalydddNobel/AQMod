using AQMod.Content;
using AQMod.Content.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Toggles
{
    public class Cosmicanon : ModItem
    {
        public static Color TextColor => new Color(160, 240, 255, 255);

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.accessory = true;
            item.rare = AQItem.RarityOmegaStarite;
            item.value = Item.buyPrice(gold: 60);
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "BloodMoonsPrevented", AQMod.GetText("BloodMoonsPrevented", CosmicanonWorldData.BloodMoonsPrevented)));
            tooltips.Add(new TooltipLine(mod, "Activity", "(" + AQMod.GetText(MiscWorldInfo.bloodMoonDisabled ? "Active" : "Inactive") + ")") { overrideColor = TextColor });
        }

        public override bool UseItem(Player player)
        {
            MiscWorldInfo.bloodMoonDisabled = !MiscWorldInfo.bloodMoonDisabled;
            if (MiscWorldInfo.bloodMoonDisabled)
            {
                AQMod.BroadcastMessage("Mods.AQMod.Cosmicanon.True", TextColor);
            }
            else
            {
                AQMod.BroadcastMessage("Mods.AQMod.Cosmicanon.False", TextColor);
            }
            return true;
        }
    }
}