using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Content.Tiles.PollutedOcean.Scrap;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Graphics.Animations;
using Terraria.GameContent.Drawing;

namespace Aequus.Content.Tiles.Furniture.Trash;

public class TrashFurniture : UnifiedFurniture {
    public override int DustType => DustID.Iron;
    public override Vector3 LightRGB => new Vector3(0.6f, 0.5f, 0.1f) * Main.rand.NextFloat(0.9f, 1f);

    public override void Load() {
        var flameInfo = new InstancedFurnitureLighted.FlameInfo(null, new Vector2(0f, 0f), Color.White, 1);
        Mod.AddContent(new InstancedFurnitureBathtub(this));
        Mod.AddContent(new InstancedFurnitureBed(this, new()));
        Mod.AddContent(new InstancedFurnitureBookcase(this));
        Mod.AddContent(new InstancedFurnitureChair(this));
        Mod.AddContent(new TrashCandelabra(this));
        Mod.AddContent(new TrashCandle(this));
        Mod.AddContent(new TrashClock(this));
        Mod.AddContent(new TrashChandelier(this));
        Mod.AddContent(new InstancedFurnitureChest(this, "ChestSmall"));
        Mod.AddContent(new InstancedFurnitureDoor(this));
        Mod.AddContent(new InstancedFurnitureDresser(this));
        Mod.AddContent(new TrashLamp(this));
        Mod.AddContent(new TrashLantern(this));
        Mod.AddContent(new InstancedFurniturePiano(this));
        Mod.AddContent(new InstancedFurnitureSink(this));
        Mod.AddContent(new InstancedFurnitureSofa(this));
        Mod.AddContent(new InstancedFurnitureTable(this));
        Mod.AddContent(new InstancedFurnitureWorkBench(this));
    }

    public override void AddRecipes(ModTile Tile, ModItem Item, Recipe Recipe) {
        Recipe.ReplaceItem(ItemID.Wood, ScrapBlock.Item.Type);
        Recipe.ReplaceItem(ItemID.Torch, ModContent.GetInstance<TrashTorch>().Item.Type);

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
