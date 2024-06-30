using Aequus.Core.Structures;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Utilities;
using tModLoaderExtended.Terraria.GameContent.Creative;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Core.ContentGeneration;

internal class InstancedCampfireTile(string name, string texture) : InstancedModTile(name, texture) {
    public ModItem DropItem { get; protected set; }

    public Vector3 LightRGB { get; set; }

    public override void Load() {
        DropItem = new InstancedTileItem(this, journeyOverride: new JourneySortByTileId(TileID.Campfire));

        Mod.AddContent(DropItem);
    }

    public override void SetStaticDefaults() {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.Campfire[Type] = true;

        DustType = -1;
        AdjTiles = [TileID.Campfire];
        VanillaFallbackOnModDeletion = TileID.Campfire;

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Campfire, 0));
        TileObjectData.newTile.StyleLineSkip = 9;
        PreAddTileObjectData();
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(254, 121, 2), Language.GetText("ItemName.Campfire"));
    }

    protected virtual void PreAddTileObjectData() {
        Main.tileWaterDeath[Type] = true;
    }

    public override void NearbyEffects(int i, int j, bool closer) {
        Tile tile = Main.tile[i, j];
        TileObjectData data = TileObjectData.GetTileData(tile);
        if (tile.TileFrameY < data.CoordinateFullHeight) {
            Main.SceneMetrics.HasCampfire = true;
        }
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;

        int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override bool RightClick(int i, int j) {
        SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
        ToggleCampfire(i, j);
        return true;
    }

    public override void HitWire(int i, int j) {
        ToggleCampfire(i, j);
    }

    private static void ToggleCampfire(int i, int j) {
        Tile tile = Main.tile[i, j];
        TileObjectData data = TileObjectData.GetTileData(tile);
        if (tile.TileFrameY < data.CoordinateFullHeight) {
            TileHelper.AdjustTileFrame(i, j, new(0, data.CoordinateFullHeight));
        }
        else {
            TileHelper.AdjustTileFrame(i, j, new(0, -data.CoordinateFullHeight));
        }
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        if (++frameCounter >= 4) {
            frameCounter = 0;
            frame = ++frame % 8;
        }
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
        Tile tile = Main.tile[i, j];
        TileObjectData data = TileObjectData.GetTileData(tile);
        if (tile.TileFrameY < data.CoordinateFullHeight) {
            frameYOffset = Main.tileFrame[type] * data.CoordinateFullHeight;
        }
        else {
            frameYOffset = data.CoordinateFullHeight * 7;
        }
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (Main.gamePaused || !Main.instance.IsActive) {
            return;
        }
        if (!Lighting.UpdateEveryFrame || new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) == 0) {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 0 && Main.rand.NextBool(3) && (Main.drawToScreen && Main.rand.NextBool(4) || !Main.drawToScreen)) {
                Dust dust = Dust.NewDustDirect(new Vector2(i * 16 + 2, j * 16 - 4), 4, 8, DustID.Smoke, 0f, 0f, 100);
                if (tile.TileFrameX == 0) {
                    dust.position.X += Main.rand.Next(8);
                }

                if (tile.TileFrameX == 36) {
                    dust.position.X -= Main.rand.Next(8);
                }

                dust.alpha += Main.rand.Next(100);
                dust.velocity *= 0.2f;
                dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
            }
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        if (Main.tile[i, j].TileFrameY >= 36) {
            return;
        }

        // Campfires have slight light pulsing
        float pulse = Main.rand.Next(28, 42) * 0.005f;
        pulse += (270 - Main.mouseTextColor) / 700f;

        Vector3 color = LightRGB;
        r = color.X + pulse;
        g = color.Y + pulse;
        b = color.Z + pulse;
    }
}

/// <summary>A subtype of <see cref="InstancedCampfireTile"/> which inherits traits from <see cref="UnifiedModTorch"/>.</summary>
internal class InstancedCampfireTorch(UnifiedModTorch parentTorch) : InstancedCampfireTile(parentTorch.Name.Replace("Torch", "Campfire"), parentTorch.NamespaceFilePath() + "/" + parentTorch.Name.Replace("Torch", "Campfire")) {
    private readonly UnifiedModTorch _modTorch = parentTorch;

    public override void Load() {
        base.Load();

        Aequus.OnAddRecipes += () => {
            DropItem.CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddIngredient(_modTorch.Type, 5)
                .Register()
                .SortAfterFirstRecipesOf(ItemID.RainbowCampfire);
        };
    }

    protected override void PreAddTileObjectData() {
        base.PreAddTileObjectData();
        if (_modTorch.AllowWaterPlacement) {
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        LightRGB = _modTorch.LightColor;
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }
}

/// <summary>A subtype of <see cref="InstancedCampfireTile"/> which inherits traits from <see cref="UnifiedFurniture"/>.</summary>
internal class InstancedFurnitureCampfire(UnifiedFurniture parent) : InstancedCampfireTile($"{parent.Name}Campfire", $"{parent.Texture}Campfire"), IAddRecipes, IModItemProvider {
    public readonly UnifiedFurniture Parent = parent;

    public override string LocalizationCategory => Parent.LocalizationCategory;

    public void AddRecipes(Mod mod) {
        Parent.AddRecipes(this, DropItem,
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.Wood, 10)
                .AddIngredient(ItemID.Torch, 5)
        );
    }

    ModItem IModItemProvider.Item => DropItem;
}