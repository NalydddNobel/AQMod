using Aequus.Core.Structures;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;
using tModLoaderExtended.Terraria.ModLoader;
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

    internal (T, ModItem) AddContent<T>(T furnitureObject) where T : ModTile, IModItemProvider {
        Mod.AddContent(furnitureObject);
        return (furnitureObject, furnitureObject.Item);
    }

    /// <summary>Allows you to edit recipes added by furniture.</summary>
    /// <param name="Tile">The furniture tile.</param>
    /// <param name="Item">The furniture item.</param>
    /// <param name="Recipe">The template recipe. Uses wood and various other materials (like Silk for beds, Torches for Candles, Water Buckets for Sinks, Iron and Glass for Clocks, etc.)</param>
    public virtual void AddRecipes(ModTile Tile, ModItem Item, Recipe Recipe) {
        Recipe.Register();
    }
}

internal abstract class InstancedFurniture(UnifiedFurniture parent, string suffix) : InstancedModTile($"{parent.Name}{suffix}", $"{parent.Texture}{suffix}"), IAddRecipes, IModItemProvider {
    public readonly UnifiedFurniture Parent = parent;

    public readonly string Suffix = suffix;

    [CloneByReference]
    public ModItem DropItem { get; protected set; }

    public int NextStyleWidth { get; private set; }
    public int NextStyleHeight { get; private set; }

    public override string LocalizationCategory => Parent.LocalizationCategory;

    public abstract void AddRecipes();

    public virtual void PreAddTileObjectData() {
        NextStyleWidth = TileObjectData.newTile.CoordinateFullWidth;
        NextStyleHeight = TileObjectData.newTile.CoordinateFullHeight;
    }

    public override void Load() {
        DropItem = new InstancedTileItem(this);
        Mod.AddContent(DropItem);
    }

    public override void NumDust(int x, int y, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    ModItem IModItemProvider.Item => DropItem;

    void IAddRecipes.AddRecipes(Mod mod) {
        AddRecipes();
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

    // Common Wire-hit code
    public override void HitWire(int i, int j) {
        TileHelper.LightToggle(i, j, NextStyleWidth);
    }

    // Common flame drawing
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

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
        DropItem.CreateRecipe()
            .AddIngredient(ItemID.Wood, 5)
            .AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.WorkBenches)
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

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 4)
                .AddIngredient(ItemID.Torch, 1)
                .AddTile(TileID.WorkBenches)
        );
    }
}

internal class InstancedFurnitureChair(UnifiedFurniture parent, string suffix = "Chair") : InstancedFurniture(parent, suffix) {
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

        if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance)) {
            player.GamepadEnableGrappleCooldown();
            player.sitting.SitDown(player, i, j);
        }

        return true;
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;

        if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance)) {
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

internal class InstancedFurnitureChandelier(UnifiedFurniture parent, FlameInfo info) : InstancedFurnitureLighted(parent, "Chandelier", info) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.newTile.Origin = new Point16(1, 0);
        TileObjectData.newTile.StyleHorizontal = true;
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        AddMapEntry(CommonColor.MapTorch, Language.GetText("MapObject.Chandelier"));
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 4)
                .AddIngredient(ItemID.Torch, 4)
                .AddIngredient(ItemID.Chain, 1)
                .AddTile(TileID.Anvils)
        );
    }
}

[Autoload(false)]
internal class InstancedFurnitureChest(UnifiedFurniture parent, string suffix = "Chest") : UnifiedModChest, IModItemProvider {
    public readonly UnifiedFurniture Parent = parent;

    public override string Name => $"{Parent.Name}{suffix}";
    public override string Texture => $"{Parent.Texture}{suffix}";

    public override string LocalizationCategory => Parent.LocalizationCategory;

    public override void Load() {
        base.Load();
        Aequus.OnAddRecipes += AddRecipe;
        void AddRecipe() {
            Parent.AddRecipes(this, DropItem,
                DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 8)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.WorkBenches)
            );
        }
    }

    public override void SafeSetStaticDefaults() {
        DustType = Parent.DustType;
    }

    ModItem IModItemProvider.Item => DropItem;
}

internal class InstancedFurnitureClock(UnifiedFurniture parent) : InstancedFurniture(parent, "Clock") {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.Clock[Type] = true;

        DustType = Parent.DustType;
        AdjTiles = [TileID.GrandfatherClocks];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
        TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.MapWoodFurniture, Lang.GetItemName(ItemID.GrandfatherClock));
    }

    public override bool RightClick(int x, int y) {
        string text = "AM";
        double time = Main.time;
        if (!Main.dayTime) {
            time += 54000.0;
        }

        time = (time / 86400.0) * 24.0;
        time = time - 7.5 - 12.0;
        if (time < 0.0) {
            time += 24.0;
        }

        if (time >= 12.0) {
            text = "PM";
        }

        int intTime = (int)time;
        double deltaTime = time - intTime;
        deltaTime = (int)(deltaTime * 60.0);
        string text2 = string.Concat(deltaTime);
        if (deltaTime < 10.0) {
            text2 = "0" + text2;
        }

        if (intTime > 12) {
            intTime -= 12;
        }

        if (intTime == 0) {
            intTime = 12;
        }

        Main.NewText($"Time: {intTime}:{text2} {text}", 255, 240, 20);
        return true;
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddIngredient(ItemID.Wood, 10)
                .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureDoor(UnifiedFurniture parent) : InstancedFurniture(parent, "Door") {
    public InstancedFurnitureDoorOpen Open { get; protected set; }

    public override void Load() {
        base.Load();

        Open = new InstancedFurnitureDoorOpen(this);
        Mod.AddContent(Open);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.NotReallySolid[Type] = true;
        TileID.Sets.DrawsWalls[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.OpenDoorID[Type] = Open.Type;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

        DustType = Parent.DustType;
        AdjTiles = [TileID.ClosedDoor];

        AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("MapObject.Door"));

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.ClosedDoor, 0));
        PreAddTileObjectData();
        TileObjectData.addTile(Type);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = 1;
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = DropItem.Type;
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 6)
                .AddTile(TileID.WorkBenches)
        );
    }

    public class InstancedFurnitureDoorOpen(InstancedFurnitureDoor openParent) : ModTile {
        public InstancedFurnitureDoor Closed = openParent;

        public override string Name => $"{Closed.Parent.Name}DoorOpen";
        public override string Texture => $"{Closed.Parent.Texture}DoorOpen";

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoSunLight[Type] = true;
            TileID.Sets.HousingWalls[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.CloseDoorID[Type] = Closed.Type;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            DustType = Closed.DustType;
            AdjTiles = [TileID.OpenDoor];
            RegisterItemDrop(Closed.DropItem.Type, 0);

            AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("MapObject.Door"));

            //TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.OpenDoor, 0));

            // Do it manually since copying doesnt work :(
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 0);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 1);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 2);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = 1;
        }

        public override void MouseOver(int i, int j) {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = Closed.DropItem.Type;
        }
    }
}

internal class InstancedFurnitureDresser(UnifiedFurniture parent) : InstancedFurniture(parent, "Dresser") {
    public override void SetStaticDefaults() {
        Main.tileSolidTop[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileTable[Type] = true;
        Main.tileContainer[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.BasicDresser[Type] = true;
        TileID.Sets.AvoidedByNPCs[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.IsAContainer[Type] = true;
        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

        AdjTiles = [TileID.Dressers];
        DustType = Parent.DustType;

        AddMapEntry(CommonColor.MapWoodFurniture, CreateMapEntryName(), MapChestName);

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
        TileObjectData.newTile.AnchorInvalidTiles = [
            TileID.MagicalIceBlock,
            TileID.Boulder,
            TileID.BouncyBoulder,
            TileID.LifeCrystalBoulder,
            TileID.RollingCactus
        ];
        TileObjectData.newTile.LavaDeath = false;
        PreAddTileObjectData();
        TileObjectData.addTile(Type);
    }

    public override LocalizedText DefaultContainerName(int frameX, int frameY) {
        return CreateMapEntryName();
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY) {
        width = 3;
        height = 1;
        extraY = 0;
    }

    public override bool RightClick(int i, int j) {
        Player player = Main.LocalPlayer;
        int left = Main.tile[i, j].TileFrameX / 18;
        left %= 3;
        left = i - left;
        int top = j - Main.tile[i, j].TileFrameY / 18;
        if (Main.tile[i, j].TileFrameY == 0) {
            Main.CancelClothesWindow(true);
            Main.mouseRightRelease = false;
            player.CloseSign();
            player.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = "";
            if (Main.editChest) {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }
            if (player.editedChestName) {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                if (left == player.chestX && top == player.chestY && player.chest != -1) {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                    Main.stackSplit = 600;
                }
            }
            else {
                player.piggyBankProjTracker.Clear();
                player.voidLensChest.Clear();
                int chestIndex = Chest.FindChest(left, top);
                if (chestIndex != -1) {
                    Main.stackSplit = 600;
                    if (chestIndex == player.chest) {
                        player.chest = -1;
                        Recipe.FindRecipes();
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else if (chestIndex != player.chest && player.chest == -1) {
                        player.OpenChest(left, top, chestIndex);
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                    }
                    else {
                        player.OpenChest(left, top, chestIndex);
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                    Recipe.FindRecipes();
                }
            }
        }
        else {
            Main.playerInventory = false;
            player.chest = -1;
            Recipe.FindRecipes();
            player.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = "";
            Main.interactedDresserTopLeftX = left;
            Main.interactedDresserTopLeftY = top;
            Main.OpenClothesWindow();
        }
        return true;
    }

    public void MouseOverAny(Player player, int i, int j) {
        Tile tile = Main.tile[i, j];
        int left = i;
        int top = j;
        left -= tile.TileFrameX % 54 / 18;
        if (tile.TileFrameY % 36 != 0) {
            top--;
        }
        int chestIndex = Chest.FindChest(left, top);
        player.cursorItemIconID = -1;
        if (chestIndex < 0) {
            player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
        }
        else {
            string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);

            if (Main.chest[chestIndex].name != "") {
                player.cursorItemIconText = Main.chest[chestIndex].name;
            }
            else {
                player.cursorItemIconText = defaultName;
            }
            if (player.cursorItemIconText == defaultName) {
                player.cursorItemIconID = DropItem.Type;
                player.cursorItemIconText = "";
            }
        }
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
    }

    public override void MouseOverFar(int i, int j) {
        Player player = Main.LocalPlayer;
        MouseOverAny(player, i, j);
        if (player.cursorItemIconText == "") {
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = 0;
        }
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        MouseOverAny(player, i, j);
        if (Main.tile[i, j].TileFrameY > 0) {
            player.cursorItemIconID = ItemID.FamiliarShirt;
            player.cursorItemIconText = "";
        }
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        Chest.DestroyChest(i, j);
    }

    public static string MapChestName(string name, int i, int j) {
        int left = i;
        int top = j;
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX % 36 != 0) {
            left--;
        }

        if (tile.TileFrameY != 0) {
            top--;
        }

        int chest = Chest.FindChest(left, top);
        if (chest < 0) {
            return Language.GetTextValue("LegacyDresserType.0");
        }

        if (Main.chest[chest].name == "") {
            return name;
        }

        return name + ": " + Main.chest[chest].name;
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 16)
                .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureLamp(UnifiedFurniture parent, FlameInfo info) : InstancedFurnitureLighted(parent, "Lamp", info) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        PreAddTileObjectData();
        TileObjectData.addTile(Type);
        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        AddMapEntry(CommonColor.MapTorch, Language.GetText("MapObject.Lamp"));
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
        DropItem.CreateRecipe()
            .AddIngredient(ItemID.Torch, 1)
            .AddIngredient(ItemID.Wood, 3)
            .AddTile(TileID.Sawmill)
        );
    }
}

internal class InstancedFurnitureLantern(UnifiedFurniture parent, FlameInfo info) : InstancedFurnitureLighted(parent, "Lantern", info) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.newTile.StyleHorizontal = true;
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        AddMapEntry(CommonColor.MapTorch, Language.GetText("MapObject.Lantern"));
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 6)
                .AddIngredient(ItemID.Torch, 1)
                .AddTile(TileID.WorkBenches)
        );
    }
}

/// <param name="parent"></param>
/// <param name="flags">Used to determine what liquids this tile counts as. By default, sinks count as a source of Water.</param>
internal class InstancedFurnitureSink(UnifiedFurniture parent, InstancedFurnitureSink.SinkSource flags = InstancedFurnitureSink.SinkSource.Water) : InstancedFurniture(parent, "Sink") {
    public enum SinkSource : byte {
        Water = 1 << 0,
        Lava = 1 << 1,
        Honey = 1 << 2,
        Shimmer = 1 << 3,
    }

    public readonly SinkSource Flags = flags;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.CountsAsWaterSource[Type] = Flags.HasFlag(SinkSource.Water);
        TileID.Sets.CountsAsHoneySource[Type] = Flags.HasFlag(SinkSource.Honey);
        TileID.Sets.CountsAsLavaSource[Type] = Flags.HasFlag(SinkSource.Lava);
        TileID.Sets.CountsAsShimmerSource[Type] = Flags.HasFlag(SinkSource.Shimmer);
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("MapObject.Sink"));
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
        );
    }
}

internal class InstancedFurnitureTable(UnifiedFurniture parent, string suffix = "Table") : InstancedFurniture(parent, suffix) {
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

internal class InstancedFurnitureWorkBench(UnifiedFurniture parent) : InstancedFurniture(parent, "WorkBench") {
    public override void SetStaticDefaults() {
        Main.tileTable[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IgnoredByNpcStepUp[Type] = true;

        DustType = Parent.DustType;
        AdjTiles = [TileID.WorkBenches];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
        TileObjectData.newTile.CoordinateHeights = [18];
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

        AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("ItemName.WorkBench"));
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 10)
        );
    }
}


/// <summary>Subtype of <see cref="InstancedFurnitureChair"/></summary>
internal class InstancedFurnitureSofa(UnifiedFurniture parent) : InstancedFurnitureChair(parent, "Sofa") {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.CanBeSatOnForPlayers[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
        AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("ItemName.Sofa"));
    }

    public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) {
        Tile tile = Framing.GetTileSafely(i, j);
        Player player = Main.LocalPlayer;

        info.DirectionOffset = 0;
        float offset = 0f;
        if (tile.TileFrameX < 17 && player.direction == 1)
            offset = 8f;
        if (tile.TileFrameX < 17 && player.direction == -1)
            offset = -8f;
        if (tile.TileFrameX > 34 && player.direction == 1)
            offset = -8f;
        if (tile.TileFrameX > 34 && player.direction == -1)
            offset = 8f;
        info.VisualOffset = new Vector2(offset, 0f);
        info.TargetDirection = player.direction;

        info.AnchorTilePosition.X = i;
        info.AnchorTilePosition.Y = j;

        if (tile.TileFrameY % NextStyleHeight == 0) {
            info.AnchorTilePosition.Y++;
        }
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;

        if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance)) {
            return;
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = DropItem.Type;
    }

    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 5)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile(TileID.WorkBenches)
        );
    }
}

/// <summary>Subtype of <see cref="InstancedFurnitureTable"/>.</summary>
internal class InstancedFurniturePiano(UnifiedFurniture parent) : InstancedFurnitureTable(parent, "Piano") {
    public override void AddRecipes() {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient(ItemID.Wood, 8)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
        );
    }
}
