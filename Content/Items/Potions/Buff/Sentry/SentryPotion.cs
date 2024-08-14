using Aequus.Common.ContentTemplates;
using Aequus.Items.Materials.Fish;
using Terraria.Localization;

namespace Aequus.Content.Items.Potions.Buff.Sentry;

public class SentryPotion() : UnifiedBuffPotion(Duration: 28800) {
    public static readonly int IncreaseMaxTurretsBy = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(IncreaseMaxTurretsBy);
    public override LocalizedText EmpoweredTooltip => base.Tooltip.WithFormatArgs(IncreaseMaxTurretsBy * 2);

    public override void Load() {
        base.Load();
        Mod.AddContent(EmpoweredBuff);
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        DrinkColors = [
            new Color(208, 101, 32, 0),
            new Color(241, 216, 109, 0),
            new Color(138, 76, 31, 0),
        ];
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient<IcebergFish>()
            .AddIngredient(ItemID.Shiverthorn)
            .AddTile(TileID.Bottles)
            .Register();
    }

    public override void UpdateBuff(Player player, ref int buffIndex) {
        player.maxTurrets += IncreaseMaxTurretsBy;
    }
}