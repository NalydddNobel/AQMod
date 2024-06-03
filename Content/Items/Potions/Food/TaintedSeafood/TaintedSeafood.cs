using Aequus.Content.Fishing;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Items.Potions.Food.TaintedSeafood;

public class TaintedSeafood : ModItem {
    public static readonly List<int> BuffOptions = new();
    public static int MinimumBuffTime { get; set; } = 5;
    public static int MaximumBuffTime { get; set; } = 10;

    public static LocalizedText BuffTimeRangeText { get; private set; }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
        this.StaticDefaultsToFood(Color.Teal, Color.DarkGreen);

        BuffOptions.Add(BuffID.WellFed);
        BuffOptions.Add(BuffID.WellFed2);
        BuffOptions.Add(BuffID.WellFed3);
        BuffOptions.Add(ModContent.BuffType<FoodPoisoningDebuff>());

        BuffTimeRangeText = Language.GetOrRegister("Mods.Aequus.Items.CommonTooltips.BuffTimeRange");

        ItemSets.ShimmerTransformToItem[Type] = ItemID.CookedFish;
    }

    public override void SetDefaults() {
        Item.DefaultToFood(10, 10, foodbuff: 0, foodbuffduration: MinimumBuffTime * 3600);
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 2);
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
    }

    public override bool? UseItem(Player player) {
        if (Main.myPlayer == player.whoAmI) {
            // Manually remove other well fed buffs
            for (int i = 0; i < Player.MaxBuffs; i++) {
                if (BuffSets.IsFedState[player.buffType[i]]) {
                    player.DelBuff(i);
                    i--;
                }
            }

            int buff = Main.rand.Next(BuffOptions);
            player.AddBuff(buff, Main.rand.Next(MinimumBuffTime, MaximumBuffTime) * 3600);
        }

        return true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        foreach (var t in tooltips) {
            if (t.Mod == "Terraria" && t.Name == "BuffTime") {
                t.Text = BuffTimeRangeText.Format(MinimumBuffTime, MaximumBuffTime);
            }
        }
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(FishInstantiator.Piraiba)
            .AddTile(TileID.CookingPots)
            .Register()
            .DisableDecraft();
    }

    public override void Unload() {
        BuffOptions.Clear();
        BuffTimeRangeText = null;
    }
}
