using Aequus.Common.Buffs;
using Aequus.Common.ContentTemplates;
using Aequus.Content.Systems.PotionAffixes.Empowered;
using Terraria.Localization;

namespace Aequus.Content.Items.Potions.Buff.Sentry;

public class SentryPotion() : UnifiedBuffPotion(Duration: 28800) {
    public static readonly int IncreaseMaxTurretsBy = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(IncreaseMaxTurretsBy);

    public override void Load() {
        base.Load();
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        DrinkColors = [
            new Color(208, 101, 32, 0),
            new Color(241, 216, 109, 0),
            new Color(138, 76, 31, 0),
        ];

        Instance<EmpoweredDatabase>().Add(Buff, Tooltip: this.GetLocalization("EmpoweredDescription"));
        AequusBuff.AddPotionConflict(Item.buffType, BuffID.Summoning);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
#if POLLUTED_OCEAN
            .AddIngredient<Fishing.Fish.Killifish>()
#else
            .AddIngredient<global::Aequus.Items.Materials.Fish.IcebergFish>()
#endif
            .AddIngredient(ItemID.Shiverthorn)
            .AddTile(TileID.Bottles)
            .Register();
    }

    public override void UpdateBuff(Player player, ref int buffIndex) {
        player.maxTurrets += IncreaseMaxTurretsBy;
    }
}