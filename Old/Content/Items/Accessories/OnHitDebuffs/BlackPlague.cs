using Aequus.Old.Content.Items.Materials.Energies;

namespace Aequus.Old.Content.Items.Accessories.OnHitDebuffs;

[AutoloadEquip(EquipType.HandsOn)]
public class BlackPlague : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.sellPrice(gold: 5);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        AequusPlayer aequus = player.GetModPlayer<AequusPlayer>();
        aequus.accBoneRing++;
        aequus.accBlackPhial++;
        player.magmaStone = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<PhoenixRing>()
            .AddIngredient<BlackPhial>()
            .AddIngredient(EnergyMaterial.Organic.Type)
            .AddTile(TileID.TinkerersWorkbench)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.FireGauntlet);
    }
}