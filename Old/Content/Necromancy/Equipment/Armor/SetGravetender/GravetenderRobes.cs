﻿using Aequus.Content.DataSets;
using Aequus.Old.Common.PlayerLayers;
using System.Linq;
using Terraria.Localization;

namespace Aequus.Old.Content.Necromancy.Equipment.Armor.SetGravetender;

[LegacyName("NecromancerRobe", "SeraphimRobes")]
[AutoloadEquip(EquipType.Body)]
public class GravetenderRobes : ModItem {
    public static int GhostLifespanIncrease { get; set; } = 1800;
    public static int GhostSlotsIncrease { get; set; } = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Seconds(GhostLifespanIncrease), GhostSlotsIncrease);

    public override void SetStaticDefaults() {
        ForceDrawShirt.BodyShowShirt.Add(Item.bodySlot);
    }

    public override void SetDefaults() {
        Item.defense = 3;
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 30);
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<AequusPlayer>().ghostSlotsMax++;
        player.GetModPlayer<AequusPlayer>().ghostLifespan += 1800;
    }

    public override void AddRecipes() {
        foreach (int rottenChunk in ItemTypeVariantMetadata.RottenChunk.Where(i => i.ValidEntry)) {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 80)
                .AddIngredient(rottenChunk, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Condition.InGraveyard)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.GravediggerShovel)
                .DisableDecraft();
        }
    }
}