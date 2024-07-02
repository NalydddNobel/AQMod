using Aequus.Core.ContentGeneration;
using Aequus.Core.Entities.Items.Components;
using Terraria.Localization;

namespace Aequus.Old.Content.Fishing.Poppers;

public class CrimsonPopper : UnifiedModBait, IModifyFishingPower {
    public static float IncreasedFishingPowerInCrimson { get; set; } = 0.2f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(XLanguage.Percent(IncreasedFishingPowerInCrimson));

    public override void SetDefaults() {
        Item.bait = 40;
        Item.value = Item.sellPrice(silver: 10);
        Item.rare = ItemRarityID.LightRed;
        Item.width = 6;
        Item.height = 6;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
    }

    public void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel) {
        if (player.ZoneCrimson) {
            fishingLevel += IncreasedFishingPowerInCrimson;
        }
    }
}