using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Content.Tiles.PollutedOcean.Scrap;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Graphics.Animations;
using Terraria.GameContent.Drawing;

namespace Aequus.Content.Tiles.Furniture.Trash;

public class TrashFurniture : UnifiedFurniture {
    public override int DustType => DustID.Iron;
    public override Vector3 LightRGB => new Vector3(0.6f, 0.5f, 0.1f) * Main.rand.NextFloat(0.9f, 1f);

    #region Instance Caches
    public ModTile Campfire { get; private set; }
    public ModItem CampfireItem { get; private set; }
    #endregion

    public override void Load() {
        var flameInfo = new InstancedFurnitureLighted.FlameInfo(null, new Vector2(0f, 0f), Color.White, 1);
        AddContent(new InstancedFurnitureBathtub(this));
        AddContent(new InstancedFurnitureBed(this, new()));
        AddContent(new InstancedFurnitureBookcase(this));
        AddContent(new InstancedFurnitureChair(this));
        AddContent(new TrashCandelabra(this));
        AddContent(new TrashCandle(this));
        AddContent(new TrashClock(this));
        AddContent(new TrashChandelier(this));
        AddContent(new InstancedFurnitureChest(this, "ChestSmall"));
        AddContent(new InstancedFurnitureDoor(this));
        AddContent(new InstancedFurnitureDresser(this));
        AddContent(new TrashLamp(this));
        AddContent(new TrashLantern(this));
        AddContent(new InstancedFurniturePiano(this));
        AddContent(new InstancedFurnitureSink(this));
        AddContent(new InstancedFurnitureSofa(this));
        AddContent(new InstancedFurnitureTable(this));
        AddContent(new InstancedFurnitureWorkBench(this));
        (Campfire, CampfireItem) = AddContent(new TrashCampfire(this));
    }

    public override void AddRecipes(ModTile Tile, ModItem Item, Recipe Recipe) {
        Recipe.ReplaceItem(ItemID.Wood, ScrapBlock.Item.Type);

        // Requires the Trash Compactor for crafting (except the work bench)
        if (Recipe.requiredTile.Count > 0) {
            Recipe.requiredTile.Clear();
            Recipe.AddTile(ModContent.TileType<TrashCompactor>());
        }

        base.AddRecipes(Tile, Item, Recipe);
    }

    public static bool CheckLightFlicker(int i, int j, int styleWidth) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX > styleWidth) {
            return true;
        }

        int left = i - tile.TileFrameX / 18;
        int top = j - tile.TileFrameY / 18;

        return AnimationSystem.TryGet(left, top, out AnimationTrashFurnitureFlicker flicker) && flicker.NoLight <= 0;
    }

    public static bool DrawLightFlickerTile(SpriteBatch spriteBatch, int Type, Texture2D flameTexture, int i, int j, int styleWidth) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX > styleWidth) {
            return true;
        }

        int left = i - tile.TileFrameX / 18;
        int top = j - tile.TileFrameY / 18;

        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Color lightColor = GetTileColor(i, j, tile);

        if (!AnimationSystem.TryGet(left, top, out AnimationTrashFurnitureFlicker flicker)) {
            flicker = new AnimationTrashFurnitureFlicker(Type);
            AnimationSystem.TileAnimations[new(i, j)] = flicker;
        }

        frame.X += flicker.NoLight == 0 ? 0 : styleWidth;

        if (TileDrawing.IsVisible(tile)) {
            spriteBatch.Draw(GetTileTexture(i, j, tile), drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        if (flicker.NoLight > 0) {
            return false;
        }

        spriteBatch.Draw(flameTexture, drawCoordinates, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        return false;
    }
}
