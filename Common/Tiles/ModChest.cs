using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.ID;

namespace Aequus.Common.Tiles;

public abstract class ModChest : ModTile {
    private LocalizedText _nameCache;

    public ModItem Item { get; private set; }

    public int FrameWidth { get; private set; }
    public int FrameHeight { get; private set; }

    public override void Load() {
        Item = new InstancedTileItem(this);
        Mod.AddContent(Item);
    }

    public sealed override void SetStaticDefaults() {
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
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(FindEmptyChest, -1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(AfterPlacement_Hook, -1, 0, false);
        TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

        SafeSetStaticDefaults();

        FrameWidth = TileObjectData.newTile.CoordinateFullWidth;
        FrameHeight = TileObjectData.newTile.CoordinateFullHeight;
        TileObjectData.addTile(Type);
    }
    public virtual void SafeSetStaticDefaults() { }

    protected virtual int FindEmptyChest(int x, int y, int type, int style, int direction, int alternate) {
        return Chest.FindEmptyChest(x, y, type, style, direction, alternate);
    }
    protected virtual int AfterPlacement_Hook(int x, int y, int type, int style, int direction, int alternate) {
        return Chest.AfterPlacement_Hook(x, y, type, style, direction, alternate);
    }

    protected string MapChestName(string name, int i, int j) {
        GetChestLocation(i, j, out var left, out var top);

        int chest = Chest.FindChest(left, top);
        if (chest < 0) {
            return Language.GetTextValue("LegacyChestType.0");
        }

        if (Main.chest[chest].name == "") {
            return name;
        }

        return name + ": " + Main.chest[chest].name;
    }

    public override LocalizedText DefaultContainerName(int frameX, int frameY) {
        return _nameCache ??= Item.DisplayName;
    }

    public override ushort GetMapOption(int i, int j) {
        return 0;
        //return (ushort)(Main.tile[i, j].TileFrameX / FrameWidth);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override bool IsLockedChest(int i, int j) {
        return Main.tile[i, j].TileFrameX / FrameWidth == 1;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        Chest.DestroyChest(i, j);
    }

    public virtual bool UnlockChest(int i, int j, int left, int top, Player player) {
        return true;
    }

    public override bool RightClick(int i, int j) {
        var player = Main.LocalPlayer;
        Main.mouseRightRelease = false;
        var tile = Main.tile[i, j];
        GetChestHoverLocation(i, j, Main.MouseWorld, in tile, out int chestX, out int chestY);

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

        var isLocked = Chest.IsLocked(chestX, chestY);
        if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked) {
            if (chestX == player.chestX && chestY == player.chestY && player.chest >= 0) {
                player.chest = -1;
                Recipe.FindRecipes();
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            else {
                NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, chestX, chestY);
                Main.stackSplit = 600;
            }
        }
        else {
            if (isLocked) {
                if (UnlockChest(i, j, chestX, chestY, player) && Chest.Unlock(chestX, chestY)) {
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, chestX, chestY);
                    }
                }
            }
            else {
                int chest = Chest.FindChest(chestX, chestY);
                if (chest >= 0) {
                    Main.stackSplit = 600;
                    if (chest == player.chest) {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else {
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        player.OpenChest(chestX, chestY, chest);
                    }

                    Recipe.FindRecipes();
                }
            }
        }

        return true;
    }

    public virtual int HoverItem(int i, int j, int left, int top) {
        return Item.Type;
    }

    public override void MouseOver(int i, int j) {
        var player = Main.LocalPlayer;
        var tile = Main.tile[i, j];
        GetChestHoverLocation(i, j, Main.MouseWorld, in tile, out int chestX, out int chestY);

        var chest = Chest.FindChest(chestX, chestY);
        player.cursorItemIconID = -1;
        if (chest < 0) {
            player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
        }
        else {
            string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
            if (player.cursorItemIconText == defaultName) {
                player.cursorItemIconID = HoverItem(i, j, chestX, chestY);
                player.cursorItemIconText = "";
            }
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
    }

    public override void MouseOverFar(int i, int j) {
        MouseOver(i, j);
        var player = Main.LocalPlayer;
        if (player.cursorItemIconText == "") {
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = 0;
        }
    }

    protected void DrawBasicGlowmask(int i, int j, SpriteBatch spriteBatch, Texture2D texture, Color color) {
        var tile = Main.tile[i, j];
        if (tile.IsTileInvisible) {
            return;
        }

        try {
            GetChestLocation(i, j, out var left, out var top);
            int chest = Chest.FindChest(left, top);
            spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + TileHelper.DrawOffset, new Rectangle(tile.TileFrameX, 38 * (chest == -1 ? 0 : Main.chest[chest].frame) + tile.TileFrameY, 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        catch {
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void GetChestLocation(int i, int j, out int chestX, out int chestY) {
        var tile = Main.tile[i, j];
        GetChestLocation(i, j, in tile, out chestX, out chestY);
    }
    protected virtual void GetChestLocation(int i, int j, in Tile tileCache, out int chestX, out int chestY) {
        chestX = i - tileCache.TileFrameX % FrameWidth / 18;
        chestY = j - tileCache.TileFrameY % FrameHeight / 18;
    }
    protected void GetChestHoverLocation(int i, int j, Vector2 mouseWorld, in Tile tileCache, out int chestX, out int chestY) {
        GetChestHoverLocation(i, j, mouseWorld.X / 16.0 % 1.0, mouseWorld.Y / 16.0 % 1.0, in tileCache, out chestX, out chestY);
    }
    protected virtual void GetChestHoverLocation(int i, int j, double mouseX, double mouseY, in Tile tileCache, out int chestX, out int chestY) {
        GetChestLocation(i, j, in tileCache, out chestX, out chestY);
    }

    protected void SendChestUpdate(int x, int y, int style) {
        NetMessage.SendData(MessageID.ChestUpdates, number: 100, number2: x, number3: y, number4: style, number5: 0, number6: Type);
    }
}