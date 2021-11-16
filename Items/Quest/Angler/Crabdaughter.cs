using AQMod.Common;
using AQMod.Content.Quest.Angler.FishingLocations;

namespace AQMod.Items.Quest.Angler
{
    public class Crabdaughter : AnglerQuestItem
    {
        public override string TextKey => "QuestFish.Crabdaughter";

        public override IFishingLocation FishingLocation => new BeachFishing(chance: 8);

        public override bool IsAnglerQuestAvailable() => WorldDefeats.DownedCrabson;
    }
}