using Aequu2.Old.Content.Items.Materials.Energies;

namespace Aequu2.Old.Content.Items.Accessories.OnHitDebuffs;

[AutoloadEquip(EquipType.HandsOn)]
public class BlackPlague : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.sellPrice(gold: 5);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        Aequu2Player Aequu2 = player.GetModPlayer<Aequu2Player>();
        Aequu2.accBoneRing++;
        Aequu2.accBlackPhial++;
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