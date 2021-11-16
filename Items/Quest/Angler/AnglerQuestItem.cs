using AQMod.Content.Quest.Angler.FishingLocations;
using AQMod.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Quest.Angler
{
    /// <summary>
    /// A little class used to make fishing quest additions easier.
    /// TODO: MAKE IT SUPPORT OTHER LIQUID TYPES
    /// </summary>
    public abstract class AnglerQuestItem : ModItem
    {
        public abstract string TextKey { get; }
        public abstract IFishingLocation FishingLocation { get; }

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
            description = AQText.ModText(TextKey).Value;
            catchLocation = AQText.ModText(FishingLocation.TextKey).Value;
        }
    }
}