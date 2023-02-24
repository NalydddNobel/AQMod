using Aequus.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.OccultistNPC.Shop
{
    public class GhostlyGrave : ModItem
    {
        public static Color TextColor => new Color(211, 200, 200, 255);

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item8;
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
            tooltips.Insert(tooltips.GetIndex("JourneyResearch"), new TooltipLine(Mod, "Activity", "(" + TextHelper.GetTextValue(Main.LocalPlayer.Aequus().ghostTombstones ? "Active" : "Inactive") + ")") { OverrideColor = TextColor });
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.Aequus();
            aequus.ghostTombstones = !aequus.ghostTombstones;
            if (Main.myPlayer == player.whoAmI)
            {
                Main.NewText(TextHelper.GetTextValue("GhostlyGrave." + (aequus.ghostTombstones ? "True" : "False")), TextColor);
            }
            return true;
        }
    }
}