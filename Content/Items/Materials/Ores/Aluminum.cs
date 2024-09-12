using Aequus.Common.ContentTemplates;
using Aequus.Common.Items;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Items.Materials.Ores;

public class Aluminum : UnifiedOre {
    public readonly int OresPerBar = 4;

    public LocalizedText? CanMineAluminumTootipHint { get; private set; }

    public override void Load() {
        CanMineAluminumTootipHint = this.GetLocalization("CanMineHint");

        Rarity = ItemRarityID.Blue;
        DustType = DustID.Silver;
        Color = Color.Gray;
        BarPrice = Item.sellPrice(silver: 20);
        OrePrice = BarPrice / OresPerBar;

        OreMetalDetectorPriority = (short)(Main.tileOreFinderPriority[TileID.Platinum] + 10); // Higher priority than Platinum, Lower than Demonite/Crimtane

        OreMiningResist = 1.5f;
        OreMiningRequirement = 45; // Requires silver pickaxe.

        base.Load();
    }

    protected override void AddRecipes() {
        BarItem.CreateRecipe()
            .AddIngredient(OreItem.Type, OresPerBar)
            .AddTile(TileID.Furnaces)
            .Register();
    }

    [Gen.AequusItem_ModifyTooltips]
    internal static void AddAluminumMiningTip(Item item, List<TooltipLine> tooltips) {
#if POLLUTED_OCEAN
        if (item.pick >= 45 && item.pick < 55) {
            tooltips.AddTooltip(new TooltipLine(Aequus.Instance, "AluminumHint", Instance<Aluminum>().CanMineAluminumTootipHint!.Value));
        }
#endif
    }
}
