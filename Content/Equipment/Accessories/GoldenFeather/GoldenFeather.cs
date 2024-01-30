using Aequus.Common.Items;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.GoldenFeather;

[LegacyName("BloodCrystal", "BloodCurcleav")]
public class GoldenFeather : ModItem {
    public static System.Int32 RespawnTimeAmount { get; set; } = -300;
    public static System.Int32 LifeRegenerationAmount { get; set; } = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Seconds(-RespawnTimeAmount), ExtendLanguage.Decimals(LifeRegenerationAmount / 2f));

    public virtual System.Int32 BuffType => ModContent.BuffType<GoldenFeatherBuff>();

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, System.Boolean hideVisual) {
        player.GetModPlayer<AequusPlayer>().accGoldenFeather = Item;
    }
}