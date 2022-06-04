using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class PickBreak : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.pickSpeed *= 2;
            player.GetModPlayer<PickBreakPlayer>().pickBreak = true;
        }
    }

    public class PickBreakPlayer : ModPlayer
    {
        public bool pickBreak;

        public override void Load()
        {
            On.Terraria.Player.PickTile += Player_PickTile;
        }

        private static void Player_PickTile(On.Terraria.Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
        {
            if (self.GetModPlayer<PickBreakPlayer>().pickBreak)
            {
                pickPower /= 2;
            }
            orig(self, x, y, pickPower);
        }

        public override void ResetEffects()
        {
            pickBreak = false;
        }
    }

    public class PickBreakTooltip : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.pick > 0 && Main.LocalPlayer.GetModPlayer<PickBreakPlayer>().pickBreak)
            {
                foreach (var t in tooltips)
                {
                    if (t.Mod == "Terraria" && t.Name == "PickPower")
                    {
                        t.Text = item.pick +
                            AequusText.ColorText("(-" + item.pick / 2 + ")", Color.Gray, alphaPulse: true) + 
                            Language.GetTextValue("LegacyTooltip.26");
                    }
                }
            }
        }
    }
}