using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Content.Tiles.PollutedOcean.Scrap;
using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Tiles.Furniture.Trash;

public class TrashFurniture : UnifiedFurniture {
    public override int DustType => DustID.Iron;
    public override Vector3 LightRGB => new Vector3(0.66f, 0.66f, 0.1f) * (Main.rand.NextBool(20) ? 0f : 1f);

    public override void Load() {
        var flameInfo = new InstancedFurnitureLighted.FlameInfo(null, new Vector2(0f, 0f), Color.White, 1);
        Mod.AddContent(new InstancedFurnitureBathtub(this));
        Mod.AddContent(new InstancedFurnitureBed(this, new()));
        Mod.AddContent(new InstancedFurnitureBookcase(this));
        Mod.AddContent(new InstancedFurnitureChair(this));
        Mod.AddContent(new InstancedFurnitureCandelabra(this, flameInfo with { FlameTexture = AequusTextures.TrashCandelabra_Flame.Preload() }));
        Mod.AddContent(new InstancedFurnitureCandle(this, flameInfo with { FlameTexture = AequusTextures.TrashCandle_Flame.Preload() }));
        Mod.AddContent(new InstancedFurnitureTable(this));
    }

    public override void AddRecipes(ModTile Tile, ModItem Item, Recipe Recipe) {
        Recipe.ReplaceItem(ItemID.Wood, ScrapBlock.Item.Type);
        Recipe.ReplaceItem(ItemID.Torch, ModContent.GetInstance<TrashTorch>().Item.Type);
        Recipe.requiredTile.Clear();
        Recipe.AddTile(ModContent.TileType<TrashCompactor>());
        base.AddRecipes(Tile, Item, Recipe);
    }
}
