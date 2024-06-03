using Aequus.Core.CodeGeneration;
using Terraria.Localization;

namespace Aequus.Content.Items.PermaPowerups.BeyondManaCrystal;

[WorkInProgress]
[Gen.AequusPlayer_SavedField<int>("consumedBeyondManaCrystals")]
public class BeyondManaCrystal : ModItem {
    public static readonly int MaxUses = 20;
    public static readonly int ManaIncrease = 5;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaIncrease);

    public override void SetStaticDefaults() {
        ItemSets.ItemNoGravity[Type] = true;
    }

    public override void SetDefaults() {
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Purple;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool? UseItem(Player player) {
        AequusPlayer aequusPlayer = player.GetModPlayer<AequusPlayer>();
        if (aequusPlayer.consumedBeyondManaCrystals >= MaxUses) {
            return false;
        }

        aequusPlayer.consumedBeyondManaCrystals++;
        return true;
    }
}