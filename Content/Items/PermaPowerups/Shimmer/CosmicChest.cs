using Aequus.Core.CodeGeneration;

namespace Aequus.Content.Items.PermaPowerups.Shimmer;

[LegacyName("GalaxyCommission", "Moro", "GhostlyGrave")]
[PlayerGen.SavedField<bool>("usedCosmicChest")]
public class CosmicChest : ModItem {
    public static float LuckIncrease { get; set; } = 0.05f;

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

    public override bool? UseItem(Player player) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        if (aequusPlayer.usedCosmicChest) {
            return false;
        }

        aequusPlayer.usedCosmicChest = true;
        return true;
    }

    [PlayerGen.PostUpdateEquips]
    internal static void OnPostUpdateEquips(AequusPlayer aequusPlayer) {
        if (aequusPlayer.usedCosmicChest) {
#if !DEBUG
            aequusPlayer.dropRolls += LuckIncrease;
#endif
        }
    }
}