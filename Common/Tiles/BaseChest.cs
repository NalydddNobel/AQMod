using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Common.Tiles;

public abstract class BaseChest : BaseFurnitureTile.Furniture {
    public override int FurnitureDust => DustID.WoodFurniture;
    public sealed override bool DieInLava => false;
    public override Color MapColor => ColorHelper.ColorFurniture;

    public abstract int ChestItem { get; }

    public override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
        Main.tileContainer[Type] = true;
        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = 1200;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileOreFinderPriority[Type] = 500;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.BasicChest[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        AdjTiles = new int[] { TileID.Containers };

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
        TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.addTile(Type);

        DustType = FurnitureDust;
        AddMapEntry(MapColor, TextHelper.GetDisplayName(ItemLoader.GetItem(ChestItem)));
    }

    public override LocalizedText DefaultContainerName(int frameX, int frameY) {
        return TextHelper.GetDisplayName(ItemLoader.GetItem(ChestItem));
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)(Main.tile[i, j].TileFrameX / 36);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override bool IsLockedChest(int i, int j) {
        return Main.tile[i, j].TileFrameX / 36 == 1;
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
            return Language.GetTextValue("LegacyChestType.0");
        }

        if (Main.chest[chest].name == "") {
            return name;
        }

        return name + ": " + Main.chest[chest].name;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        Chest.DestroyChest(i, j);
    }

    public virtual bool CheckLocked(int i, int j, int left, int top, Player player) {
        return true;
    }

    public override bool RightClick(int i, int j) {
        Player player = Main.LocalPlayer;
        Tile tile = Main.tile[i, j];
        Main.mouseRightRelease = false;
        int left = i;
        int top = j;
        if (tile.TileFrameX % 36 != 0) {
            left--;
        }

        if (tile.TileFrameY != 0) {
            top--;
        }

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

        bool isLocked = Chest.IsLocked(left, top);
        if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked) {
            if (left == player.chestX && top == player.chestY && player.chest >= 0) {
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
            if (isLocked) {
                if (CheckLocked(i, j, left, top, player) && Chest.Unlock(left, top)) {
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, left, top);
                    }
                }
            }
            else {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0) {
                    Main.stackSplit = 600;
                    if (chest == player.chest) {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else {
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        player.OpenChest(left, top, chest);
                    }

                    Recipe.FindRecipes();
                }
            }
        }

        return true;
    }

    public virtual int HoverItem(int i, int j, int left, int top) {
        return ChestItem;
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        Tile tile = Main.tile[i, j];
        int left = i;
        int top = j;
        if (tile.TileFrameX % 36 != 0) {
            left--;
        }

        if (tile.TileFrameY != 0) {
            top--;
        }

        int chest = Chest.FindChest(left, top);
        player.cursorItemIconID = -1;
        if (chest < 0) {
            player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
        }
        else {
            string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
            if (player.cursorItemIconText == defaultName) {
                player.cursorItemIconID = HoverItem(i, j, left, top);
                player.cursorItemIconText = "";
            }
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
    }

    public override void MouseOverFar(int i, int j) {
        MouseOver(i, j);
        Player player = Main.LocalPlayer;
        if (player.cursorItemIconText == "") {
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = 0;
        }
    }

    protected static void DrawBasicGlowmask(int i, int j, SpriteBatch spriteBatch, Texture2D texture, Color color) {
        var tile = Main.tile[i, j];
        if (tile.IsTileInvisible) {
            return;
        }

        try {
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0) {
                left--;
            }
            if (tile.TileFrameY != 0) {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + TileHelper.DrawOffset, new Rectangle(tile.TileFrameX, 38 * (chest == -1 ? 0 : Main.chest[chest].frame) + tile.TileFrameY, 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        catch {
        }
    }
}