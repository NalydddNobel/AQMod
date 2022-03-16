using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Toggles
{
    public class GhostlyGrave : ModItem
    {
        public static Color TextColor => new Color(211, 200, 200, 255);

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(gold: 45);
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item8;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (lightColor.R < 60)
            {
                lightColor.R = 60;
            }
            if (lightColor.G < 60)
            {
                lightColor.G = 60;
            }
            if (lightColor.B < 60)
            {
                lightColor.B = 60;
            }
            lightColor.A = 200;
            return lightColor;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "Activity", "(" + AQMod.GetText(Main.LocalPlayer.GetModPlayer<TombstonesPlayer>().disableTombstones ? "Active" : "Inactive") + ")") { overrideColor = TextColor });
        }

        public override bool UseItem(Player player)
        {
            var tombstonesPlayer = player.GetModPlayer<TombstonesPlayer>();
            tombstonesPlayer.disableTombstones = !tombstonesPlayer.disableTombstones;
            if (Main.myPlayer == player.whoAmI)
            {
                if (tombstonesPlayer.disableTombstones)
                {
                    Main.NewText(Language.GetTextValue("Mods.AQMod.GhostlyGrave.True"), TextColor);
                }
                else
                {
                    Main.NewText(Language.GetTextValue("Mods.AQMod.GhostlyGrave.False"), TextColor);
                }
            }
            return true;
        }
    }
}