using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.UI.InfoIcons {
    public class AnglerBroadcasterIcon : InfoDisplay {
        public override string DisplayValue(ref Color displayColor) {
            if (Main.anglerQuestFinished || Main.anglerQuest == -1 || Main.anglerQuest >= Main.anglerQuestItemNetIDs.Length || !NPC.AnyNPCs(NPCID.Angler)) {
                displayColor = InactiveInfoTextColor;
                return TextHelper.GetTextValue("Finished");
            }
            return Lang.GetItemNameValue(Main.anglerQuestItemNetIDs[Main.anglerQuest]);
        }

        public override bool Active() {
            return Main.LocalPlayer.Aequus().accShowQuestFish;
        }
    }
}
