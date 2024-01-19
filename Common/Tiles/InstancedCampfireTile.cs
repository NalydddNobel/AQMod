using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Aequus.Common.Tiles;

internal class InstancedCampfireTile : InstancedModTile {
    private readonly ModTorch _modTorch;
    public ModItem Item { get; private set; }

    public InstancedCampfireTile(ModTorch parentTorch) : base(parentTorch.Name.Replace("Torch", "Campfire"), parentTorch.NamespaceFilePath() + "/" + parentTorch.Name.Replace("Torch", "Campfire")) {
        _modTorch = parentTorch;
    }

    public override void Load() {
        Item = new InstancedTileItem(this)
            .WithRecipe(m => {
                m.CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddIngredient(_modTorch.Type, 5)
                .Register()
                .SortAfterFirstRecipesOf(ItemID.RainbowCampfire);
            });

        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.Campfire[Type] = true;

        DustType = -1;
        AdjTiles = new int[] { TileID.Campfire };
        VanillaFallbackOnModDeletion = TileID.Campfire;

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Campfire, 0));
        TileObjectData.newTile.StyleLineSkip = 9;
        if (_modTorch.AllowWaterPlacement) {
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
        }
        else {
            Main.tileWaterDeath[Type] = true;
        }
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(254, 121, 2), Language.GetText("ItemName.Campfire"));
    }

    public override void NearbyEffects(int i, int j, bool closer) {
        Main.SceneMetrics.HasCampfire = true;
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
        int topX = i - tile.TileFrameX % 54 / 18;
        int topY = j - tile.TileFrameY % 36 / 18;

        short frameAdjustment = (short)(tile.TileFrameY >= 36 ? -36 : 36);

        for (int x = topX; x < topX + 3; x++) {
            for (int y = topY; y < topY + 2; y++) {
                Main.tile[x, y].TileFrameY += frameAdjustment;

                if (Wiring.running) {
                    Wiring.SkipWire(x, y);
                }
            }
        }

        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendTileSquare(-1, topX, topY, 3, 2);
        }
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        if (++frameCounter >= 4) {
            frameCounter = 0;
            frame = ++frame % 8;
        }
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
        var tile = Main.tile[i, j];
        if (tile.TileFrameY < 36) {
            frameYOffset = Main.tileFrame[type] * 36;
        }
        else {
            frameYOffset = 252;
        }
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (Main.gamePaused || !Main.instance.IsActive) {
            return;
        }
        if (!Lighting.UpdateEveryFrame || new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) == 0) {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 0 && Main.rand.NextBool(3) && ((Main.drawToScreen && Main.rand.NextBool(4)) || !Main.drawToScreen)) {
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

        Vector3 color = _modTorch.LightColor;
        r = color.X + pulse;
        g = color.Y + pulse;
        b = color.Z + pulse;
    }
}
