using AQMod.Common.Utilities;
using AQMod.Content.World;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Toggles
{
    public class Stardrop : ModItem
    {
        public static Color TextColor => Color.HotPink * 1.25f;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = AQItem.RarityOmegaStarite + 1;
            item.value = Item.buyPrice(platinum: 1);
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item8;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (MiscWorldInfo.villagerMoveInAtNight && Main.myPlayer != -1)
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
            tooltips.Add(new TooltipLine(mod, "Activity", "(" + AQMod.GetText(MiscWorldInfo.villagerMoveInAtNight ? "Active" : "Inactive") + ")") { overrideColor = TextColor });
        }

        public override bool UseItem(Player player)
        {
            MiscWorldInfo.villagerMoveInAtNight = !MiscWorldInfo.villagerMoveInAtNight;
            if (Main.myPlayer == player.whoAmI)
            {
                if (MiscWorldInfo.villagerMoveInAtNight)
                {
                    MessageBroadcast.NewMessage(Language.GetTextValue("Mods.AQMod.Stardrop.True"), TextColor);
                }
                else
                {
                    MessageBroadcast.NewMessage(Language.GetTextValue("Mods.AQMod.Stardrop.False"), TextColor);
                }
            }
            return true;
        }
    }
}