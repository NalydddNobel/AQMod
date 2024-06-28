using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;
using static Aequus.Core.ContentGeneration.InstancedFurnitureLighted;

namespace Aequus.Core.ContentGeneration;

public abstract class UnifiedFurniture : ModTexturedType, ILocalizedModType {
    public abstract int DustType { get; }
    public abstract Vector3 LightRGB { get; }

    public override string Name => base.Name.Replace("Furniture", "");

    public string LocalizationCategory => "Tiles.Furniture";

    protected sealed override void Register() {
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    /// <summary>Allows you to edit recipes added by furniture.</summary>
    /// <param name="Tile">The furniture tile.</param>
    /// <param name="Item">The furniture item.</param>
    /// <param name="Recipe">The template recipe. Uses wood and various other materials (like Silk for beds, Torches for Candles, Water Buckets for Sinks, Iron and Glass for Clocks, etc.)</param>
    public virtual void AddRecipes(ModTile Tile, ModItem Item, Recipe Recipe) {
        Recipe.Register();
    }
}

internal class InstancedFurnitureItem(InstancedFurniture parent) : InstancedModItem(parent.Name, $"{parent.Texture}Item") {
    [CloneByReference]
    internal readonly InstancedFurniture Parent = parent;

    public override LocalizedText DisplayName => Parent.GetLocalization($"ItemDisplayName");
    public override LocalizedText Tooltip {
        get {
            LocalizedText tooltip = Language.GetText(Parent.GetLocalizationKey($"ItemTooltip"));
            return tooltip.Key == tooltip.Value ? LocalizedText.Empty : tooltip;
        }
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(Parent.Type);
    }

    public override void AddRecipes() {
        Parent.AddRecipes();
    }
}

internal abstract class InstancedFurniture(UnifiedFurniture parent, string suffix) : InstancedModTile($"{parent.Name}{suffix}", $"{parent.Texture}{suffix}") {
    public readonly UnifiedFurniture Parent = parent;

    public readonly string Suffix = suffix;

    [CloneByReference]
    public ModItem DropItem { get; protected set; }

    public int NextStyleWidth { get; private set; }
    public int NextStyleHeight { get; private set; }

    public override string LocalizationCategory => Parent.LocalizationCategory;

    public override void Load() {
        DropItem = new InstancedFurnitureItem(this);
        Mod.AddContent(DropItem);
    }

    public override void NumDust(int x, int y, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public abstract void AddRecipes();

    public virtual void PreAddTileObjectData() {
        NextStyleWidth = TileObjectData.newTile.CoordinateFullWidth;
        NextStyleHeight = TileObjectData.newTile.CoordinateFullHeight;
    }
}

internal abstract class InstancedFurnitureLighted(UnifiedFurniture parent, string suffix, FlameInfo info) : InstancedFurniture(parent, suffix) {
    public readonly FlameInfo Info = info;
    public readonly record struct FlameInfo(Asset<Texture2D> FlameTexture, Vector2? FlameSize = null, Color? FlameColor = null, int FlameImages = 7);

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        if (Main.tile[i, j].TileFrameX < NextStyleWidth) {
            Vector3 rgb = Parent.LightRGB;
            r = rgb.X;
            g = rgb.Y;
            b = rgb.Z;
        }
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        Main.tileLighted[Type] = true;

        if (Info.FlameTexture == null) {
            return;
        }

        ulong seed = TileHelper.GetTorchSeed(i, j);
        Vector2 drawOffset = TileHelper.DrawOffset;
        Vector2 flameSize = Info.FlameSize ?? new Vector2(1f, 1f);
        Tile tile = Main.tile[i, j];
        Color color = Info.FlameColor ?? new Color(100, 100, 100, 0);
        Texture2D flameTexture = Info.FlameTexture.Value;
        int images = Info.FlameImages;
        Vector2 drawCoordinates = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y);
        for (int k = 0; k < images; k++) {
            Vector2 offset = TileHelper.GetTorchOffset(ref seed);
            offset.X *= flameSize.X;
            offset.Y *= flameSize.Y;
            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
            spriteBatch.Draw(flameTexture, drawCoordinates + offset + drawOffset, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}

internal class InstancedFurnitureBathtub(UnifiedFurniture parent) : InstancedFurniture(parent, "Bathtub") {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
        AddMapEntry(CommonColor.MapBathtub, Lang.GetItemName(ItemID.Bathtub));

        AdjTiles = [TileID.Bathtubs];
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 14)
                .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureBed(UnifiedFurniture parent, InstancedFurnitureBed.BedInfo info) : InstancedFurniture(parent, "Bed") {
    public readonly BedInfo Info = info;

    public readonly record struct BedInfo(Vector2 SleepingOffset);

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.CanBeSleptIn[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.IsValidSpawnPoint[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

        DustType = Parent.DustType;
        AdjTiles = [TileID.Beds];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.Bed"));
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY) {
        width = 2;
        height = 2;
    }

    public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info) {
        info.VisualOffset += Info.SleepingOffset;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = 1;
    }

    public override bool RightClick(int i, int j) {
        Player player = Main.LocalPlayer;
        Tile tile = Main.tile[i, j];
        int spawnX = (i - (tile.TileFrameX / 18)) + (tile.TileFrameX >= 72 ? 5 : 2);
        int spawnY = j + 2;

        if (tile.TileFrameY % NextStyleHeight != 0) {
            spawnY--;
        }

        if (!Player.IsHoveringOverABottomSideOfABed(i, j)) {
            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance)) {
                player.GamepadEnableGrappleCooldown();
                player.sleeping.StartSleeping(player, i, j);
            }
        }
        else {
            player.FindSpawn();

            if (player.SpawnX == spawnX && player.SpawnY == spawnY) {
                player.RemoveSpawn();
                Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), CommonColor.TextInteractable);
            }
            else if (Player.CheckSpawn(spawnX, spawnY)) {
                player.ChangeSpawn(spawnX, spawnY);
                Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), CommonColor.TextInteractable);
            }
        }

        return true;
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;

        if (!Player.IsHoveringOverABottomSideOfABed(i, j)) {
            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance)) { // Match condition in RightClick. Interaction should only show if clicking it does something
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ItemID.SleepingIcon;
            }
        }
        else {
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = DropItem.Type;
        }
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureBookcase(UnifiedFurniture parent) : InstancedFurniture(parent, "Bookcase") {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileTable[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
        AddMapEntry(CommonColor.MapWoodFurniture, Lang.GetItemName(ItemID.Bookcase));

        AdjTiles = [TileID.Bookcases];
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureCandelabra(UnifiedFurniture parent, FlameInfo info) : InstancedFurnitureLighted(parent, "Candelabra", info) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        PreAddTileObjectData();
        TileObjectData.addTile(Type);
        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        AddMapEntry(CommonColor.MapTorch, Lang.GetItemName(ItemID.Candelabra));
    }

    public override void HitWire(int i, int j) {
        Tile tile = Main.tile[i, j];
        int topX = i - tile.TileFrameX / 18 % 2;
        int topY = j - tile.TileFrameY / 18 % 2;
        short frameAdjustment = (short)(tile.TileFrameX >= 36 ? -36 : 36);
        Main.tile[topX, topY].TileFrameX += frameAdjustment;
        Main.tile[topX, topY + 1].TileFrameX += frameAdjustment;
        Main.tile[topX + 1, topY].TileFrameX += frameAdjustment;
        Main.tile[topX + 1, topY + 1].TileFrameX += frameAdjustment;
        Wiring.SkipWire(topX, topY);
        Wiring.SkipWire(topX, topY + 1);
        Wiring.SkipWire(topX + 1, topY);
        Wiring.SkipWire(topX + 1, topY + 1);
        NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
        DropItem.CreateRecipe()
            .AddIngredient(ItemID.Wood, 14)
            .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureCandle(UnifiedFurniture parent, FlameInfo info) : InstancedFurnitureLighted(parent, "Candle", info) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidWithTop | AnchorType.Table, TileObjectData.newTile.Width, 0);
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        AddMapEntry(CommonColor.MapTorch, Lang.GetItemName(ItemID.Candle));
    }

    public override void MouseOver(int i, int j) {
        var player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = DropItem.Type;
    }

    public override bool RightClick(int i, int j) {
        WorldGen.KillTile(i, j);
        return true;
    }

    public override void HitWire(int i, int j) {
        Tile tile = Main.tile[i, j];
        int topX = i - tile.TileFrameX / 18 % 1;
        int topY = j - tile.TileFrameY / 18 % 1;
        short frameAdjustment = (short)(tile.TileFrameX >= 18 ? -18 : 18);
        Main.tile[topX, topY].TileFrameX += frameAdjustment;
        Wiring.SkipWire(topX, topY);
        NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 14)
                .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureChair(UnifiedFurniture parent) : InstancedFurniture(parent, "Chair") {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.CanBeSatOnForNPCs[Type] = true;
        TileID.Sets.CanBeSatOnForPlayers[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

        DustType = Parent.DustType;
        AdjTiles = [TileID.Chairs];

        AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("MapObject.Chair"));

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        PreAddTileObjectData();
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
    }

    public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) {
        Tile tile = Framing.GetTileSafely(i, j);

        //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
        //info.visualOffset = Vector2.Zero; // Defaults to (0,0)
        info.TargetDirection = tile.TileFrameX != 0 ? 1 : -1;

        info.AnchorTilePosition.X = i;
        info.AnchorTilePosition.Y = j;

        if (tile.TileFrameY % NextStyleHeight == 0) {
            info.AnchorTilePosition.Y++;
        }
    }

    public override bool RightClick(int i, int j) {
        Player player = Main.LocalPlayer;

        if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance)) { // Avoid being able to trigger it from long range
            player.GamepadEnableGrappleCooldown();
            player.sitting.SitDown(player, i, j);
        }

        return true;
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;

        if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance)) { // Match condition in RightClick. Interaction should only show if clicking it does something
            return;
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = DropItem.Type;

        if (Main.tile[i, j].TileFrameX / 18 < 1) {
            player.cursorItemIconReversed = true;
        }
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 4)
                .AddTile(TileID.WorkBenches)
        );
    }
}

internal class InstancedFurnitureTable(UnifiedFurniture parent) : InstancedFurniture(parent, "Table") {
    public override void SetStaticDefaults() {
        Main.tileTable[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IgnoredByNpcStepUp[Type] = true;

        DustType = Parent.DustType;
        AdjTiles = [TileID.Tables];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

        AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("MapObject.Table"));
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 8)
                .AddTile(TileID.WorkBenches)
        );
    }
}
