#if POLLUTED_OCEAN
using Aequus.Common.Items;
using Aequus.Common.Utilities;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Items.Consumable.Food.TaintedSeafood;

public class TaintedSeafood : ModItem {
    public static readonly List<int> BuffOptions = [];

    public static readonly int MinimumBuffTime = 18000;
    public static readonly int MaximumBuffTime = 36000;

    public static LocalizedText? BuffTimeRangeText { get; private set; }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
        this.StaticDefaultsToFood(Color.Teal, Color.DarkGreen);

        BuffOptions.Add(BuffID.WellFed);
        BuffOptions.Add(BuffID.WellFed2);
        BuffOptions.Add(BuffID.WellFed3);
        BuffOptions.Add(ModContent.BuffType<FoodPoisoningDebuff>());

        BuffTimeRangeText = ALanguage.GetText("Items.CommonTooltips.BuffTimeRange");

        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.CookedFish;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.CookedFish);
        Item.buffType = 0;
        Item.buffTime = MinimumBuffTime;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
    }

    public override bool? UseItem(Player player) {
        if (Main.myPlayer == player.whoAmI) {
            // Manually remove other well fed buffs
            for (int i = 0; i < Player.MaxBuffs; i++) {
                if (BuffID.Sets.IsFedState[player.buffType[i]]) {
                    player.DelBuff(i);
                    i--;
                }
            }

            int buff = Main.rand.Next(BuffOptions);
            player.AddBuff(buff, Main.rand.Next(MinimumBuffTime / 3600, MaximumBuffTime / 3600) * 3600);
        }

        return true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        foreach (var t in tooltips) {
            if (t.Mod == "Terraria" && t.Name == "BuffTime") {
                t.Text = BuffTimeRangeText!.Format(ALanguage.Minutes(MinimumBuffTime), ALanguage.Minutes(MaximumBuffTime));
            }
        }
    }

    public override void Unload() {
        BuffOptions.Clear();
        BuffTimeRangeText = null;
    }
}
#endif
