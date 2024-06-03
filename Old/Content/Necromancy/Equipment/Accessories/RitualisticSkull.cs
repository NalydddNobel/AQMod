using Aequus.Core.CodeGeneration;

namespace Aequus.Old.Content.Necromancy.Equipment.Accessories;

[Gen.AequusPlayer_ResetField<bool>("accRitualSkull")]
public class RitualisticSkull : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accRitualSkull = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.PygmyNecklace)
            .AddIngredient(ItemID.SpectreBar, 8)
            .AddIngredient(ItemID.SoulofFright, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PapyrusScarab);
    }

    [Gen.AequusPlayer_PostUpdateEquips]
    internal static void OnPostUpdateEquips(Player player, AequusPlayer aequusPlayer) {
        if (aequusPlayer.accRitualSkull) {
            aequusPlayer.ghostSlots += player.maxMinions;
            player.maxMinions = 1;
        }
    }
}