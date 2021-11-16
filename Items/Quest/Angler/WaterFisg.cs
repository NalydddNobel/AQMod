using AQMod.Content.Quest.Angler.FishingLocations;

namespace AQMod.Items.Quest.Angler
{
    public class WaterFisg : AnglerQuestItem
    {
        public override string TextKey => "QuestFish.WaterFisg";

        public override IFishingLocation FishingLocation => new AnywhereFishing(chance: 15);
    }
}