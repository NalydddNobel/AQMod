using Aequu2.Core.Entities.Items.Dedications;
using System.Collections.Generic;
using Terraria.Localization;
using tModLoaderExtended.Terraria.GameContent;

namespace Aequu2.Content.Dedicated.BeyondCoin;

public class ShimmerCoin : ModItem, IShimmerOverride {
    public static int TimesUsed { get; set; }
    public static int MaxTimesUsed { get; set; } = 5;
    public static float Effectiveness => TimesUsed / (float)MaxTimesUsed;

    public override void SetDefaults() {
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
        DedicationRegistry.RegisterSubItem(ModContent.GetInstance<BeyondPlatinumCoin>(), this);
    }

    public override bool? UseItem(Player player) {
        if (TimesUsed >= MaxTimesUsed) {
            return false;
        }

        TimesUsed++;
        return true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.AddTooltip(new TooltipLine(Mod, "Uses", Language.GetTextValue("Mods.Aequu2.Items.CommonTooltips.UsedXOfXTimes", TimesUsed, MaxTimesUsed)));
    }

    bool? IShimmerOverride.IsTransformLocked(int type) {
        return NPC.downedMoonlord ? null : false;
    }
}
