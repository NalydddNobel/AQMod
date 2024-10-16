﻿using Aequus.Items.Weapons.Ranged.Bows.CrusadersCrossbow;
using Terraria.Localization;

namespace Aequus.Content.Necromancy.Accessories.SpiritKeg;

[LegacyName("BloodiedBucket")]
public class SaivoryKnife : ModItem {
    /// <summary>
    /// Default Value: 3600 (1 minute)
    /// </summary>
    public static int GhostLifespan { get; set; } = 3600;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(GhostLifespan / 3600);

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CrusadersCrossbow>();
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.Aequus().ghostLifespan += GhostLifespan;
    }
}