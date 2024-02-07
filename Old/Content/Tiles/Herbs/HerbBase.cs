using System.Collections.Generic;
using Terraria.GameContent.Metadata;
using Terraria.ObjectData;

namespace Aequus.Old.Content.Tiles.Herbs;

public abstract class HerbBase : ModTile {
    protected short FrameWidth { get; private set; }

    public override bool CanPlace(int i, int j) {
        Tile tile = Main.tile[i, j];
        return !tile.HasTile || tile.TileType != Type;
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
        if (i % 2 == 1) {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        int growthStage = Main.tile[i, j].TileFrameX / FrameWidth;
        Player closestPlayer = Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)];
        Item item = closestPlayer.HeldItem;

        GetDropParams(growthStage, closestPlayer, item, out int plant, out int seeds);

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
    protected abstract void GetDropParams(int growthStage, Player closestPlayer, Item playerHeldItem, out int plant, out int seeds);
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
        if (tile.TileFrameX == 0) {
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
}
