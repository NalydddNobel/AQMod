using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class WaterFisg : ModItem
    {
        public override void SetDefaults()
        {
            item.questItem = true;
            item.maxStack = 1;
            item.width = 26;
            item.height = 26;
            item.uniqueStack = true;
            item.rare = ItemRarityID.Quest;
        }

        public override bool IsQuestFish()
        {
            return true;
        }

        public override bool IsAnglerQuestAvailable() => true;

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = Language.GetTextValue("Mods.AQMod.QuestFish.WaterFisg");
            catchLocation = Language.GetTextValue("Mods.AQMod.FishingLocation.Anywhere");
        }
    }
}