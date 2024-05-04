using Aequus.Common;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.GoldenFeather;

[LegacyName("BloodCrystal", "BloodCurcleav")]
public class GoldenFeather : ModItem {
    public static int RespawnTimeAmount { get; set; } = -300;
    public static int LifeRegenerationAmount { get; set; } = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Seconds(-RespawnTimeAmount), ExtendLanguage.Decimals(LifeRegenerationAmount / 2f));

    public virtual int BuffType => ModContent.BuffType<GoldenFeatherBuff>();

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = Commons.Rare.SkyMerchantShopItem;
        Item.value = Commons.Cost.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accGoldenFeather = Item;
    }
}