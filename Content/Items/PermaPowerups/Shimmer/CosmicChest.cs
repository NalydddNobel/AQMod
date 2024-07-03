using Aequu2.Core.CodeGeneration;

namespace Aequu2.Content.Items.PermaPowerups.Shimmer;

[LegacyName("GalaxyCommission", "Moro", "GhostlyGrave")]
[Gen.AequusPlayer_SavedField<bool>("usedCosmicChest")]
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
        var Aequu2Player = player.GetModPlayer<AequusPlayer>();
        if (Aequu2Player.usedCosmicChest) {
            return false;
        }

        Aequu2Player.usedCosmicChest = true;
        return true;
    }

    [Gen.AequusPlayer_PostUpdateEquips]
    internal static void OnPostUpdateEquips(AequusPlayer Aequu2Player) {
        if (Aequu2Player.usedCosmicChest) {
#if !DEBUG
            Aequu2Player.dropRolls += LuckIncrease;
#endif
        }
    }
}