using Terraria.ModLoader;

namespace AQMod.Items.QuestFish
{
    public class WaterFisg : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.rare = -11;
            item.questItem = true;
        }

        public override bool IsQuestFish() => true;

        public override bool IsAnglerQuestAvailable() => true;

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = "Its strange to say this, but im awfully thirsty. " +
                "I saw this clear fish filled with water IN the water. " +
                "Kinda strange right? Now go catch it.";
            catchLocation = "Caught anywhere";
        }
    }
}
