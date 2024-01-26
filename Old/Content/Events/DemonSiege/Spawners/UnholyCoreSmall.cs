using Aequus.Content.Events.DemonSiege;

namespace Aequus.Old.Content.Events.DemonSiege.Spawners;

public class UnholyCoreSmall : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
        // Hack which makes this item get consumed after the event is over, instead of converting into another item
        GoreNestConversions.Register(Type, Type);
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void AddRecipes() {
        Recipe.Create(ItemID.Vilethorn)
            .AddIngredient(ItemID.DemoniteBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.CrimsonRod)
            .AddIngredient(ItemID.CrimtaneBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.BallOHurt)
            .AddIngredient(ItemID.DemoniteBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.TheRottedFork)
            .AddIngredient(ItemID.CrimtaneBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.BandofStarpower)
            .AddIngredient(ItemID.DemoniteBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.PanicNecklace)
            .AddIngredient(ItemID.CrimtaneBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.ShadowOrb)
            .AddIngredient(ItemID.DemoniteBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.CrimsonHeart)
            .AddIngredient(ItemID.CrimtaneBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.Musket)
            .AddIngredient(ItemID.DemoniteBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
        Recipe.Create(ItemID.TheUndertaker)
            .AddIngredient(ItemID.CrimtaneBar, 8)
            .AddIngredient(Type, 3)
            .AddTile(TileID.DemonAltar)
            .Register();
    }
}