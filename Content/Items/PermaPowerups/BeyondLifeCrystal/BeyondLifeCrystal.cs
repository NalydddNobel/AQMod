using AequusRemake.Core.CodeGeneration;
using Terraria.Localization;

namespace AequusRemake.Content.Items.PermaPowerups.BeyondLifeCrystal;

[WorkInProgress]
[Gen.AequusPlayer_SavedField<int>("consumedBeyondLifeCrystals")]
public class BeyondLifeCrystal : ModItem {
    public static readonly int MaxUses = 20;
    public static readonly int LifeIncrease = 5;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeIncrease);

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
        AequusPlayer AequusRemakePlayer = player.GetModPlayer<AequusPlayer>();
        if (AequusRemakePlayer.consumedBeyondLifeCrystals >= MaxUses) {
            return false;
        }

        AequusRemakePlayer.consumedBeyondLifeCrystals++;
        return true;
    }
}