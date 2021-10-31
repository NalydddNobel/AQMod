using AQMod.Common;
using Terraria.ModLoader;

namespace AQMod.Items.QuestFish
{
    public class Crabdaughter : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.rare = -11;
            item.questItem = true;
        }

        public override bool IsQuestFish() => true;

        public override bool IsAnglerQuestAvailable() => WorldDefeats.DownedCrabson;

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = "Seems like new fish have entered the ocean recently, one of these fish seems to be related this massive crab with CLAMS as CLAWS. " +
                "The only thing that makes me think it's related is that it's a clam WITH CLAWS. " +
                "It's an amazing sight so catch one for me.";
            catchLocation = "Caught at the ocean";
        }
    }
}