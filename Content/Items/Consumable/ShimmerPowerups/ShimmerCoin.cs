using Aequus.Common.Items;
using Aequus.Common.Items.Dedications;
using Aequus.Common.Recipes;
using Aequus.Common.Utilities.Helpers;
using Aequus.Content.Items.Materials.BeyondCoin;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Items.Consumable.ShimmerPowerups;

public class ShimmerCoin : ModItem {
    //public static readonly int MaxTimesUsed = 5;
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
        AequusRecipes.Overrides.ShimmerTransformLocks[Type] = Condition.DownedMoonLord;
        DedicationRegistry.RegisterSubItem(ModContent.GetInstance<BeyondPlatinumCoin>(), this);
    }

    public override bool? UseItem(Player player) {
        if (TimesUsed >= MaxTimesUsed) {
            return false;
        }

        TimesUsed++;

        if (Main.netMode != NetmodeID.MultiplayerClient) {
            NetworkText text = this.GetLocalization("Announcement")
                .ToNetworkText(player.name, TimesUsed, MaxTimesUsed);

            WorldGen.BroadcastText(text, CommonColor.TextEvent);
        }
        return true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.AddTooltip(new TooltipLine(Mod, "Uses", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.UsedXOfXTimes", TimesUsed, MaxTimesUsed)));
    }
}
