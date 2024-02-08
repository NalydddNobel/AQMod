using Aequus.Core.ContentGeneration;
using System.Collections.Generic;
using Terraria.GameContent.Metadata;
using Terraria.ObjectData;

namespace Aequus.Old.Content.Tiles.Herbs;

public abstract class ModHerb : ModTile {
    public const int STAGE_IMMATURE = 0;
    public const int STAGE_MATURE = 1;
    public const int STAGE_BLOOMING = 2;

    protected short FrameWidth { get; private set; }

    public ModItem SeedItem { get; private set; }

    public override void Load() {
        SeedItem = new InstancedSeedItem(this);
        Mod.AddContent(SeedItem);
    }

    public override bool CanPlace(int i, int j) {
        Tile tile = Main.tile[i, j];
        return !tile.HasTile || tile.TileType != Type;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        if (GetGrowthStage(i, j) == STAGE_BLOOMING) {
            tileFrameX = (short)(FrameWidth * STAGE_BLOOMING);
        }
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
        if (i % 2 == 1) {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        int growthStage = GetGrowthStage(i, j);
        Player closestPlayer = Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)];
        Item item = closestPlayer.HeldItem;

        int seeds = SeedItem.Type;
        GetDropParams(growthStage, closestPlayer, item, out int plant);

        if (closestPlayer.active && (item.type == ItemID.StaffofRegrowth || item.type == ItemID.AcornAxe)) {
            if (growthStage == 1) {
                yield return new Item(plant);
                yield return new Item(seeds, Main.rand.Next(1, 3));
            }
            if (growthStage == 2) {
                yield return new Item(plant, Main.rand.Next(1, 3));
                yield return new Item(seeds, Main.rand.Next(1, 6));
            }
        }
        else {
            if (growthStage == 1) {
                yield return new Item(plant);
            }
            if (growthStage == 2) {
                yield return new Item(plant);
                yield return new Item(seeds, Main.rand.Next(1, 4));
            }
        }
    }

    /// <param name="growthStage">The growth stage (style) of this plant.</param>
    /// <param name="closestPlayer">The closest player to the plant.</param>
    /// <param name="playerHeldItem">The player's held item.</param>
    /// <param name="plant">The plant item id to drop.</param>
    /// <param name="seeds">The seed item id to drop.</param>
    protected abstract void GetDropParams(int growthStage, Player closestPlayer, Item playerHeldItem, out int plant);
    /// <param name="growthStage">The growth stage (style) of this plant.</param>
    /// <param name="closestPlayer">The closest player to the plant.</param>
    /// <param name="playerHeldItem">The player's held item.</param>
    /// <param name="staffOfRegrowth">If the player is holding the Staff of Regrowth or Axe of Regrowth.</param>
    /// <param name="plant">The plant item id to drop.</param>
    /// <param name="seeds">The seed item id to drop.</param>
    /// <param name="plantStack">The amount of plants to drop.</param>
    /// <param name="seedStack">The amount of seeds to drop.</param>
    protected virtual void GetDropStacks(int growthStage, Player closestPlayer, Item playerHeldItem, bool staffOfRegrowth, int plant, int seeds, out int plantStack, out int seedStack) {
        plantStack = growthStage switch {
            1 => 1,
            2 => !staffOfRegrowth ? 1 : Main.rand.Next(1, 3),
            _ => 0
        };
        seedStack = growthStage switch {
            1 => !staffOfRegrowth ? 0 : Main.rand.Next(1, 3),
            2 => !staffOfRegrowth ? Main.rand.Next(1, 4) : Main.rand.Next(1, 6),
            _ => 0
        };
    }

    public override void RandomUpdate(int i, int j) {
        Tile tile = Main.tile[i, j];

        // Will naturally take a bit to grow from stage 1 to 2
        // Stage 2 to 3 is determined by the derived class
        if (tile.TileFrameX == 0 && WorldGen.genRand.NextBool(10)) {
            tile.TileFrameX += FrameWidth;
            NetMessage.SendTileSquare(-1, i, j, 1);
        }
    }

    public sealed override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileCut[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileWaterDeath[Type] = false;
        Main.tileObsidianKill[Type] = true;

        TileID.Sets.SwaysInWindBasic[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileID.Sets.IgnoredInHouseScore[Type] = true;
        TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

        DustType = 39;
        HitSound = SoundID.Grass;

        TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
        // Aequus' planter boxes will automatically detect that this anchors to TileID.PlanterBox
        // and add our planter box to this array accordingly.
        TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.ClayPot, TileID.PlanterBox };

        SafeSetStaticDefaults();

        FrameWidth = (short)TileObjectData.newTile.CoordinateFullWidth;

        TileObjectData.addTile(Type);
    }

    protected abstract void SafeSetStaticDefaults();

    protected abstract bool IsBlooming(int i, int j);

    public int GetGrowthStage(int i, int j) {
        int style = Main.tile[i, j].TileFrameX / 18;

        return style switch {
            STAGE_BLOOMING => STAGE_BLOOMING,
            STAGE_MATURE => IsBlooming(i, j) ? STAGE_BLOOMING : STAGE_MATURE,
            _ => STAGE_IMMATURE
        };
    }

    internal class InstancedSeedItem : InstancedModItem {
        [CloneByReference]
        private readonly ModTile _parent;

        public InstancedSeedItem(ModHerb tile) : base(tile.Name + "Seeds", tile.Texture + "Seeds") {
            _parent = tile;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.DaybloomSeeds);
            Item.createTile = _parent.Type;
            Item.placeStyle = 0;
        }
    }
}
