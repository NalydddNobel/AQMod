using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Content.Tiles.PollutedOcean.Scrap;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Graphics.Animations;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Furniture.Trash;

public class TrashFurniture : UnifiedFurniture {
    public override int DustType => DustID.Iron;
    public override Vector3 LightRGB => new Vector3(0.6f, 0.5f, 0.1f);

    public override void Load() {
        var flameInfo = new InstancedFurnitureLighted.FlameInfo(null, new Vector2(0f, 0f), Color.White, 1);
        Mod.AddContent(new InstancedFurnitureBathtub(this));
        Mod.AddContent(new InstancedFurnitureBed(this, new()));
        Mod.AddContent(new InstancedFurnitureBookcase(this));
        Mod.AddContent(new InstancedFurnitureChair(this));
        Mod.AddContent(new TrashCandelabra(this));
        Mod.AddContent(new TrashCandle(this));
        Mod.AddContent(new TrashClock(this));
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

public class AnimationTrashFurnitureFlicker(int AnchorTileType) : ITileAnimation {
    public int NoLight;

    public bool Update(int x, int y) {
        Tile tile = Main.tile[x, y];

        if (NoLight == 0 && Main.rand.NextBool(720)) {
            NoLight = Main.rand.Next(20);
        }
        else if (NoLight > 0) {
            NoLight--;
        }

        return tile.HasTile && tile.TileType == AnchorTileType && Cull2D.Tile(x, y);
    }
}

internal class TrashCandelabra(UnifiedFurniture parent) : InstancedFurnitureCandelabra(parent, default) {
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX > NextStyleWidth) {
            return;
        }

        int left = i - tile.TileFrameX / 18;
        int top = j - tile.TileFrameY / 18;

        if (!AnimationSystem.TryGet(left, top, out AnimationTrashFurnitureFlicker flicker) || flicker.NoLight > 0) {
            return;
        }

        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX > NextStyleWidth) {
            return true;
        }

        int left = i - tile.TileFrameX / 18;
        int top = j - tile.TileFrameY / 18;

        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Color lightColor = Lighting.GetColor(i, j);

        if (!AnimationSystem.TryGet(left, top, out AnimationTrashFurnitureFlicker flicker)) {
            flicker = new AnimationTrashFurnitureFlicker(Type);
            AnimationSystem.TileAnimations[new(left, top)] = flicker;
        }

        frame.X += flicker.NoLight == 0 ? 0 : NextStyleWidth;

        spriteBatch.Draw(TileTexture[Type].Value, drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        if (flicker.NoLight > 0) {
            return false;
        }

        spriteBatch.Draw(AequusTextures.TrashCandelabra_Flame, drawCoordinates, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        return false;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) { }
}

internal class TrashCandle(UnifiedFurniture parent) : InstancedFurnitureCandle(parent, default) {
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX > NextStyleWidth) {
            return;
        }

        int left = i - tile.TileFrameX / 18;
        int top = j - tile.TileFrameY / 18;

        if (!AnimationSystem.TryGet(i, j, out AnimationTrashFurnitureFlicker flicker) || flicker.NoLight > 0) {
            return;
        }

        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX > NextStyleWidth) {
            return true;
        }

        int left = i - tile.TileFrameX / 18;
        int top = j - tile.TileFrameY / 18;

        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Color lightColor = Lighting.GetColor(i, j);

        if (!AnimationSystem.TryGet(left, top, out AnimationTrashFurnitureFlicker flicker)) {
            flicker = new AnimationTrashFurnitureFlicker(Type);
            AnimationSystem.TileAnimations[new(i, j)] = flicker;
        }

        frame.X += flicker.NoLight == 0 ? 0 : NextStyleWidth;

        spriteBatch.Draw(TileTexture[Type].Value, drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        if (flicker.NoLight > 0) {
            return false;
        }

        spriteBatch.Draw(AequusTextures.TrashCandle_Flame, drawCoordinates, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        return false;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) { }
}

internal class TrashClock(UnifiedFurniture parent) : InstancedFurnitureClock(parent) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.Clock[Type] = true;

        DustType = Parent.DustType;
        AdjTiles = [TileID.GrandfatherClocks];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, 3, 0);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.MapWoodFurniture, Lang.GetItemName(ItemID.GrandfatherClock));
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        Texture2D texture = GetTileTexture(i, j, tile);
        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset;
        Color lightColor = GetTileColor(i, j, tile);

        if (TileDrawing.IsVisible(tile)) {
            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, tile.TileFrameY <= 0 ? 16 : 18);
            spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        // Clock Digits inherit the paint color and light of the bottom right tile.
        if (tile.TileFrameX == 36 && tile.TileFrameY == 18) {
            DrawDigits(spriteBatch, texture, drawCoordinates, lightColor);
        }

        return false;
    }

    private void DrawDigits(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawCoordinates, Color lightColor) {
        int[] digits = Helper.GetTimeDigits(Helper.Get24HourTimeStartingAtMidnight());

        for (int k = 0; k < digits.Length; k++) {
            int digit = digits[k] - 1;
            Rectangle digitFrame = digit switch {
                -1 => new Rectangle(8, 62, 6, 10),
                _ => new Rectangle(8 * (digit % 4), 38 + 12 * (digit / 4), 6, 10),
            };
            //Rectangle digitFrame = new Rectangle(0, 38, 6, 10);
            int offset = 8 * k;
            offset += k > 1 ? 4 : 0;
            spriteBatch.Draw(texture, drawCoordinates + new Vector2(-26f + offset, 2f), digitFrame, lightColor * 2f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        spriteBatch.Draw(texture, drawCoordinates + new Vector2(-12f, 2f), new Rectangle(16, 62, 6, 10), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }
}