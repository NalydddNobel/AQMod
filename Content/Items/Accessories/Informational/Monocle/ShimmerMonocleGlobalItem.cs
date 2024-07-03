using Aequu2.DataSets;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequu2.Content.Items.Accessories.Informational.Monocle;

public sealed class ShimmerMonocleGlobalItem : GlobalItem {
    public static Dictionary<int, Func<Item, string>> CustomShimmerTip { get; private set; } = new();

    public static Color TipColor { get; set; } = Color.Lerp(Color.White, Color.BlueViolet, 0.33f);

    public override void Load() {
        CustomShimmerTip[ItemID.GelBalloon] = (i) => NPC.unlockedSlimeRainbowSpawn ? null : Language.GetTextValue("Mods.Aequu2.Items.CommonTooltips.ShimmerableToNPC", Lang.GetNPCNameValue(NPCID.TownSlimeRainbow));
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return (ItemSets.ShimmerTransformToItem[entity.type] > 0 || CustomShimmerTip.ContainsKey(entity.type)) && !ItemDataSet.ShimmerTooltipResultIgnore.Contains(ItemSets.ShimmerTransformToItem[entity.type]);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (!Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoShimmerMonocle || ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState != 0) {
            return;
        }

        int itemId = ItemSets.ShimmerCountsAsItem[item.type] != -1 ? ItemSets.ShimmerCountsAsItem[item.type] : item.type;
        if (CustomShimmerTip.TryGetValue(item.type, out var getCustomShimmerTip)) {
            var customShimmerTip = getCustomShimmerTip(item);
            if (customShimmerTip != null) {
                tooltips.Insert(tooltips.GetIndex("Material", 1), new(Mod, "Shimmerable", customShimmerTip) { OverrideColor = TipColor });
            }
        }

        if (ItemSets.ShimmerTransformToItem[item.type] <= 0 || ItemDataSet.ShimmerTooltipResultIgnore.Contains(ItemSets.ShimmerTransformToItem[item.type]) || !item.CanShimmer()) {
            return;
        }

        tooltips.Insert(tooltips.GetIndex("Material", 1), new(Mod, "Shimmerable", Language.GetTextValue("Mods.Aequu2.Items.CommonTooltips.Shimmerable", ItemSets.ShimmerTransformToItem[itemId], Lang.GetItemNameValue(ItemSets.ShimmerTransformToItem[itemId]))) { OverrideColor = TipColor });
    }
}