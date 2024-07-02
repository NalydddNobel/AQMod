using Aequus.Core.ContentGeneration;
using Aequus.Core.Entities.Items.Components;
using Terraria.Localization;

namespace Aequus.Old.Content.Fishing.Poppers;

[LegacyName("MysticPopper")]
public class HallowPopper : UnifiedModBait, IModifyFishingPower {
    public static float IncreasedFishingPowerInHallow { get; set; } = 0.4f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(XLanguage.Percent(IncreasedFishingPowerInHallow));

    public override void SetDefaults() {
        Item.bait = 20;
        Item.value = Item.sellPrice(silver: 12);
        Item.rare = ItemRarityID.LightRed;
        Item.width = 6;
        Item.height = 6;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
    }

    public void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel) {
        if (player.ZoneHallow) {
            fishingLevel += IncreasedFishingPowerInHallow;
        }
    }
}