using Terraria.ObjectData;

namespace Aequus.Tiles.Herbs;
public abstract class HerbTileBase : ModTile {
    protected virtual int FrameWidth => 26;
    protected virtual int FrameHeight => 30;

    protected short FrameShiftX => (short)(FrameWidth + 2);

    protected virtual int[] GrowableTiles => new int[] { TileID.Grass, TileID.HallowedGrass, };
    protected virtual Color MapColor => Color.White;
    protected virtual string MapName => GetType().Name;
    protected virtual int DrawOffsetY => 0;

    public virtual Vector3 GlowColor => new Vector3(1f, 1f, 1f);
    public virtual bool IsBlooming(int i, int j) {
        return true;
    }
    public virtual bool CanBeHarvestedWithStaffOfRegrowth(int i, int j) {
        return Main.tile[i, j].TileFrameX >= FrameShiftX * 2;
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileCut[Type] = true;
        Main.tileNoFail[Type] = true;
        //Main.tileAlch[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
        TileObjectData.newTile.CoordinateWidth = FrameWidth;
        TileObjectData.newTile.CoordinateHeights = new int[] { FrameHeight };
        TileObjectData.newTile.DrawYOffset = DrawOffsetY;
        TileObjectData.newTile.AnchorValidTiles = GrowableTiles;

        TileObjectData.newTile.AnchorAlternateTiles = new int[]
        {
            TileID.ClayPot,
            TileID.PlanterBox
        };

        TileObjectData.addTile(Type);

        DustType = DustID.Grass;
        HitSound = SoundID.Grass;

        AddMapEntry(MapColor, TextHelper.GetText($"MapObject.{MapName}"));
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
        if (i % 2 == 1)
            spriteEffects = SpriteEffects.FlipHorizontally;
    }

    public override bool CanPlace(int i, int j) {
        return !Main.tile[i, j].HasTile || Main.tile[i, j].TileType == Type ? CanBeHarvestedWithStaffOfRegrowth(i, j) : !Main.tileAlch[Main.tile[i, j].TileType];
    }

    public override void RandomUpdate(int i, int j) {
        if (Main.tile[i, j].TileFrameX < 28 && Main.rand.NextBool(100)) {
            Main.tile[i, j].TileFrameX += FrameShiftX;
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, i, j, 1);
            return;
        }
        bool blooming = IsBlooming(i, j);
        if (blooming) {
            if (Main.tile[i, j].TileFrameX < 56) {
                Main.tile[i, j].TileFrameX += FrameShiftX;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }
        else if (Main.tile[i, j].TileFrameX > 28) {
            Main.tile[i, j].TileFrameX -= FrameShiftX;
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, i, j, 1);
        }
    }

    protected static bool TryPlaceHerb<T>(int i, int j, int checkSize, params int[] validTile) where T : ModTile {
        int tile = ModContent.TileType<T>();
        for (int y = j - 1; y > 20; y--) {
            if (!WorldGen.InWorld(i, y, 30) || Main.tile[i, y].HasTile || !Main.tile[i, y + 1].HasUnactuatedTile || Main.tile[i, y + 1].Slope != SlopeType.Solid || Main.tile[i, y + 1].IsHalfBlock) {
                continue;
            }

            for (int k = 0; k < validTile.Length; k++) {
                if (Main.tile[i, y + 1].TileType != validTile[k]) {
                    continue;
                }

                if (TileHelper.ScanTiles(new Rectangle(i - checkSize, y - checkSize, checkSize * 2, checkSize * 2).Fluffize(20), TileHelper.HasTileAction(tile))) {
                    return false;
                }

                WorldGen.PlaceTile(i, y, tile, mute: true);
                Main.tile[i, y].CopyPaintAndCoating(Main.tile[i, y + 1]);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendTileSquare(-1, i - 1, y - 1, 3, 3);

                return true;
            }
        }
        return false;
    }
}
public class StaffOfRegrowthHerbsHelper : ILoadable {
    public void Load(Mod mod) {
        On_Player.PlaceThing_Tiles_BlockPlacementForAssortedThings += Player_PlaceThing_Tiles_BlockPlacementForAssortedThings;
    }

    private static bool Player_PlaceThing_Tiles_BlockPlacementForAssortedThings(On_Player.orig_PlaceThing_Tiles_BlockPlacementForAssortedThings orig, Player player, bool canPlace) {
        if (player.HeldItem.type == ItemID.StaffofRegrowth && Main.tile[Player.tileTargetX, Player.tileTargetY].HasTile
            && Main.tile[Player.tileTargetX, Player.tileTargetY].TileType >= TileID.Count && TileLoader.GetTile(Main.tile[Player.tileTargetX, Player.tileTargetY].TileType) is HerbTileBase herbTile) {
            if (herbTile.CanBeHarvestedWithStaffOfRegrowth(Player.tileTargetX, Player.tileTargetY)) {
                WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY);
                if (!Main.tile[Player.tileTargetX, Player.tileTargetY].HasTile && Main.netMode == NetmodeID.MultiplayerClient) {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, Player.tileTargetX, Player.tileTargetY);
                }
            }
            canPlace = true;
        }
        return orig(player, canPlace);
    }

    public void Unload() {
    }
}