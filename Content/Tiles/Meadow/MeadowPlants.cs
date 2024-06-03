using Aequus.Content.Items.Potions.Mana;
using System.Collections.Generic;
using Terraria.GameContent.Metadata;

namespace Aequus.Content.Tiles.Meadow;
public class MeadowPlants : ModTile {
    public override void SetStaticDefaults() {
        Main.tileCut[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileID.Sets.SwaysInWindBasic[Type] = true;
        TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

        AddMapEntry(new Color(79, 188, 247));

        DustType = DustID.GrassBlades;
        HitSound = SoundID.Grass;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        Tile down = Framing.GetTileSafely(i, j + 1);
        if (!down.HasUnactuatedTile || down.TileType != ModContent.TileType<MeadowGrass>()) {
            WorldGen.KillTile(i, j);
        }
        return true;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        offsetY = -2;
        height = 20;
    }

    public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance) {
        wormChance = 50;
        grassHopperChance = 12;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        switch (Main.tile[i, j].TileFrameX / 18) {
            case 8: {
                    yield return new Item(ModContent.ItemType<MeadowMushroom>());
                }
                break;

            default:
                Vector2 worldCoords = new Vector2(i, j).ToWorldCoordinates();
                Player closestPlayer = Main.player[Player.FindClosest(worldCoords, 16, 16)];
                if (closestPlayer.active) {
                    if (closestPlayer.HeldItem.type == ItemID.Sickle) {
                        yield return new Item(ItemID.Hay, Main.rand.Next(1, 2 + 1));
                    }
                }
                break;
        }
    }
}
