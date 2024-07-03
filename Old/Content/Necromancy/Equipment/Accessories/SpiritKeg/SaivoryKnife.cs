using Terraria.Localization;

namespace Aequu2.Old.Content.Necromancy.Equipment.Accessories.SpiritKeg;

[LegacyName("BloodiedBucket")]
public class SaivoryKnife : ModItem {
    /// <summary>Default Value: 3600 (1 minute)</summary>
    public static int GhostLifespan { get; set; } = 3600;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(XLanguage.Minutes(GhostLifespan));

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        //player.Aequu2().ghostLifespan += GhostLifespan;
    }
}