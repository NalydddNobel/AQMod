using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.UI
{
    public class AnglerBroadcasterIcon : InfoDisplay
    {
        [Obsolete("Helper for easier porting to 1.4.4")]
        private Color InactiveInfoTextColor => Color.Gray;

        public override void SetStaticDefaults()
        {
            //InfoName.SetDefault("{$Mods.Aequus.InfoDisplayName.AnglerBroadcasterIcon}");
        }

        public override string DisplayValue()
        {
            var displayColor = Color.White;
            if ((Main.anglerQuestFinished || Main.anglerQuest == -1 || Main.anglerQuest >= Main.anglerQuestItemNetIDs.Length || !NPC.AnyNPCs(NPCID.Angler)))
            {
                displayColor = InactiveInfoTextColor;
                return TextHelper.GetTextValue("Finished");
            }
            return Lang.GetItemNameValue(Main.anglerQuestItemNetIDs[Main.anglerQuest]);
        }

        public override bool Active()
        {
            return Main.LocalPlayer.Aequus().accShowQuestFish;
        }
    }
}
