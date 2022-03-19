using AQMod.Content.World;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Toggles
{
    public class EclipseGlasses : ModItem
    {
        public static Color TextColor => new Color(255, 175, 100, 255);

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.buyPrice(gold: 60);
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (AQWorld.Hooks.EclipsePreventionFailed)
            {
                var c = lightColor * 0.5f;
                return new Color(c.R, c.G, c.B, lightColor.A);
            }
            if (MiscWorldInfo.eclipseDisabled && Main.myPlayer != -1)
            {
                var plr = Main.LocalPlayer;
                if (plr != null && plr.talkNPC != -1 && Main.npc[plr.talkNPC].type == ModContent.NPCType<Physicist>())
                {
                    var c = lightColor * 0.5f;
                    return new Color(c.R, c.G, c.B, lightColor.A);
                }
            }
            return null;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "EclipsesPrevented", AQMod.GetText("EclipsesPrevented", MiscWorldInfo.eclipsesPrevented)));
            tooltips.Add(new TooltipLine(mod, "Activity", "(" + AQMod.GetText(MiscWorldInfo.eclipseDisabled ? "Active" : "Inactive") + ")") { overrideColor = TextColor });
            if (AQWorld.Hooks.BloodMoonPreventionFailed)
            {
                tooltips.Add(new TooltipLine(mod, "Failed", "Looks like the code which this uses didn't get applied correctly!\nWill most likely not work.") { overrideColor = Color.Gray });
            }
        }

        public override bool UseItem(Player player)
        {
            MiscWorldInfo.eclipseDisabled = !MiscWorldInfo.eclipseDisabled;
            if (MiscWorldInfo.eclipseDisabled)
            {
                AQMod.BroadcastMessage("Mods.AQMod.EclipseGlasses.True", TextColor);
            }
            else
            {
                AQMod.BroadcastMessage("Mods.AQMod.EclipseGlasses.False", TextColor);
            }
            return true;
        }
    }
}