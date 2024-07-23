using Aequus.Common.Items;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.Informational.Monocle;

public sealed class ShimmerMonocleGlobalItem : GlobalItem {
    public static readonly Dictionary<int, Func<Item, string>> CustomShimmerTip = [];
    public static readonly HashSet<int> BlacklistResult = [];

    public static Color TipColor { get; set; } = Color.Lerp(Color.White, Color.BlueViolet, 0.33f);

    public override void Load() {
        CustomShimmerTip[ItemID.GelBalloon] = (i) => NPC.unlockedSlimeRainbowSpawn ? null : Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.ShimmerableToNPC", Lang.GetNPCNameValue(NPCID.TownSlimeRainbow));
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return (ItemID.Sets.ShimmerTransformToItem[entity.type] > 0 || CustomShimmerTip.ContainsKey(entity.type)) && !BlacklistResult.Contains(ItemID.Sets.ShimmerTransformToItem[entity.type]);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (!Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoShimmerMonocle || ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState != 0) {
            return;
        }

        int itemId = ItemID.Sets.ShimmerCountsAsItem[item.type] != -1 ? ItemID.Sets.ShimmerCountsAsItem[item.type] : item.type;
        if (CustomShimmerTip.TryGetValue(item.type, out var getCustomShimmerTip)) {
            var customShimmerTip = getCustomShimmerTip(item);
            if (customShimmerTip != null) {
                tooltips.Insert(tooltips.GetIndex("Material", 1), new TooltipLine(Mod, "Shimmerable", customShimmerTip) { OverrideColor = TipColor });
            }
        }

        if (ItemID.Sets.ShimmerTransformToItem[item.type] <= 0 || BlacklistResult.Contains(ItemID.Sets.ShimmerTransformToItem[item.type]) || !item.CanShimmer()) {
            return;
        }

        tooltips.Insert(tooltips.GetIndex("Material", 1), new(Mod, "Shimmerable", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.Shimmerable", ItemID.Sets.ShimmerTransformToItem[itemId], Lang.GetItemNameValue(ItemID.Sets.ShimmerTransformToItem[itemId]))) { OverrideColor = TipColor });
    }
}