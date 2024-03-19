using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod.CalamityModSupport;

internal class CalamityMod : SupportedMod<CalamityMod> {
    public static int RarityTurqouise_12 = ItemRarityID.Purple;
    public static int RarityPureGreen_13 = ItemRarityID.Purple;
    public static int RarityDarkBlue_14 = ItemRarityID.Purple;
    public static int RarityViolet_15 = ItemRarityID.Purple;
    public static int RarityHotPink_16 = ItemRarityID.Purple;
    public static int RarityCalamityRed_17 = ItemRarityID.Purple;

    public override void SafeLoad(Mod mod) {
        ModCompatabilityFlags.RemoveExpertExclusivity = true;
    }

    public override void PostSetupContent() {
        SetRarity("Turqouise", out RarityTurqouise_12);
        SetRarity("PureGreen", out RarityPureGreen_13);
        SetRarity("DarkBlue", out RarityDarkBlue_14);
        SetRarity("Violet", out RarityViolet_15);
        SetRarity("HotPink", out RarityHotPink_16);
        SetRarity("CalamityRed", out RarityCalamityRed_17);

        static void SetRarity(string name, out int rarity) {
            if (TryFind(name, out ModRarity modRarity)) {
                rarity = modRarity.Type;
            }
            else {
                rarity = ItemRarityID.Purple;
            }
        }
    }
}