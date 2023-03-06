using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.UI
{
    public class AnglerBroadcasterIcon : InfoDisplay
    {
        public override void SetStaticDefaults()
        {
            //InfoName.SetDefault("{$Mods.Aequus.InfoDisplayName.AnglerBroadcasterIcon}");
        }

        public override string DisplayValue(ref Color displayColor)/* tModPorter Suggestion: Set displayColor to InactiveInfoTextColor if your display value is "zero"/shows no valuable information */
        {
            return (Main.anglerQuestFinished || Main.anglerQuest == -1 || Main.anglerQuest >= Main.anglerQuestItemNetIDs.Length || !NPC.AnyNPCs(NPCID.Angler))
                ? TextHelper.GetTextValue("Finished") : Lang.GetItemNameValue(Main.anglerQuestItemNetIDs[Main.anglerQuest]);
        }

        public override bool Active()
        {
            return Main.LocalPlayer.Aequus().accShowQuestFish;
        }
    }
}
