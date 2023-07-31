using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture;

public class TrappedChestBaseTile<TChestTile, TItem> : ModTile where TChestTile : BaseChest<TItem> where TItem : ModItem {
    // TODO -- Make Trapped Chests use a custom texture (Vanilla adds a little wire marker into the chest)
    public override string Texture => ModContent.GetInstance<TChestTile>().Texture;

    public override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = 1200;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileOreFinderPriority[Type] = 500;
        Main.tileLavaDeath[Type] = false;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.AvoidedByNPCs[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.BasicChestFake[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.CoordinateHeights = new int[] {
            16,
            18
        };
        TileObjectData.newTile.AnchorInvalidTiles = new int[] {
            TileID.MagicalIceBlock
        };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.addTile(Type);
        AddMapEntry(ModContent.GetInstance<TChestTile>().MapColor, TextHelper.GetItemName<TItem>());

        DustType = -1;
        AdjTiles = new int[] {
            TileID.FakeContainers,
            TileID.FakeContainers2,
        };
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

    public override ushort GetMapOption(int i, int j) {
        return (ushort)((Main.tile[i, j] != null) ? (Main.tile[i, j].TileFrameX / 36) : 0);
    }

    public override void MouseOver(int i, int j) {
        Player localPlayer = Main.LocalPlayer;
        localPlayer.noThrow = 2;
        localPlayer.cursorItemIconEnabled = true;
        localPlayer.cursorItemIconText = "";
        localPlayer.cursorItemIconID = ModContent.ItemType<TItem>();
    }

    public override bool RightClick(int i, int j) {
        Tile tile = Main.tile[i, j];
        Main.mouseRightRelease = false;
        int num = i;
        int num2 = j;
        if (tile.TileFrameX % 36 != 0) {
            num--;
        }
        if (tile.TileFrameY != 0) {
            num2--;
        }
        Animation.NewTemporaryAnimation(2, tile.TileType, num, num2);
        NetMessage.SendTemporaryAnimation(-1, 2, tile.TileType, num, num2);
        Trigger(i, j);
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            // TODO -- Send Trapped Chest opening data
        }
        return true;
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
        Tile tile = Main.tile[i, j];
        int num = i;
        int num2 = j;
        if (tile.TileFrameX % 36 != 0) {
            num--;
        }
        if (tile.TileFrameY != 0) {
            num2--;
        }
        if (Animation.GetTemporaryFrame(num, num2, out int num3)) {
            frameYOffset = 38 * num3;
        }
    }

    public static void Trigger(int i, int j) {
        Tile tile = Main.tile[i, j];
        int num = i;
        int num2 = j;
        if (tile.TileFrameX % 36 != 0) {
            num--;
        }
        if (tile.TileFrameY != 0) {
            num2--;
        }
        SoundEngine.PlaySound(SoundID.Mech, new(i * 16, j * 16));
        Wiring.TripWire(num, num2, 2, 2);
    }
}