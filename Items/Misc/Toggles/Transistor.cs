using AQMod.Content.World;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Toggles
{
    public class Transistor : ModItem
    {
        public static Color TextColor => new Color(160, 240, 255, 255);

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.buyPrice(gold: 60);
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (MiscWorldInfo.glimmerDisabled && Main.myPlayer != -1)
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
            tooltips.Add(new TooltipLine(mod, "GlimmersPrevented", AQMod.GetText("GlimmersPrevented", MiscWorldInfo.glimmersPrevented)));
            tooltips.Add(new TooltipLine(mod, "Activity", "(" + AQMod.GetText(MiscWorldInfo.glimmerDisabled ? "Active" : "Inactive") + ")") { overrideColor = TextColor });
        }

        public override bool UseItem(Player player)
        {
            MiscWorldInfo.glimmerDisabled = !MiscWorldInfo.glimmerDisabled;
            if (MiscWorldInfo.glimmerDisabled)
            {
                AQMod.BroadcastMessage("Mods.AQMod.Transistor.True", TextColor);
            }
            else
            {
                AQMod.BroadcastMessage("Mods.AQMod.Transistor.False", TextColor);
            }
            return true;
        }
    }
}