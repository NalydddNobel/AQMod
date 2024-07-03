using Aequu2.Core.CodeGeneration;

namespace Aequu2.Old.Content.Necromancy.Equipment.Accessories;

[Gen.Aequu2Player_ResetField<bool>("accRitualSkull")]
public class RitualisticSkull : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<Aequu2Player>().accRitualSkull = true;
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

    [Gen.Aequu2Player_PostUpdateEquips]
    internal static void OnPostUpdateEquips(Player player, Aequu2Player Aequu2Player) {
        if (Aequu2Player.accRitualSkull) {
            Aequu2Player.ghostSlots += player.maxMinions;
            player.maxMinions = 1;
        }
    }
}