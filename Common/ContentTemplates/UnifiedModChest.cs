using Aequus.Common.ContentTemplates.Generic;
using System.Runtime.CompilerServices;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Common.ContentTemplates;

public abstract class UnifiedModChest : ModTile {
    public ModItem Item { get; protected set; }
    internal InstancedTrappedChest? TrappedChest { get; set; }

    public int FrameWidth { get; private set; }
    public int FrameHeight { get; private set; }

    public virtual bool LoadTrappedChest => true;

    internal ChestSettings Settings = new();
    internal TileItemSettings ItemSettings = new();

    public UnifiedModChest() {
        Item = new InstancedTileItem(this/*, journeyOverride: new JourneySortByTileId(TileID.Containers2)*/, Settings: ItemSettings);
    }

    #region Custom Hooks
    protected virtual int FindEmptyChest(int x, int y, int type, int style, int direction, int alternate) {
        return Chest.FindEmptyChest(x, y, type, style, direction, alternate);
    }

    protected virtual int AfterPlacement_Hook(int x, int y, int type, int style, int direction, int alternate) {
        return Chest.AfterPlacement_Hook(x, y, type, style, direction, alternate);
    }

    public virtual bool CanUnlockChest(int i, int j, int left, int top, Player player) {
        return true;
    }

    public virtual int HoverItem(int i, int j, int left, int top) {
        return Item.Type;
    }
    #endregion

    #region Helper Methods
    protected void DrawChestTile(int i, int j, SpriteBatch spriteBatch, Texture2D texture, Color color, int frameXOffset = 0, int frameYOffset = 0) {
        var tile = Main.tile[i, j];
        if (tile.IsInvisible()) {
            return;
        }

        try {
            GetChestLocation(i, j, out var left, out var top);
            int chest = Chest.FindChest(left, top);
            Rectangle frame = new Rectangle(tile.TileFrameX + frameXOffset, 38 * (chest == -1 ? 0 : Main.chest[chest].frame) + tile.TileFrameY + frameYOffset, 16, 16);
            spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + Helper.TileDrawOffset, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
    #endregion

    #region Initalization
    public override void Load() {
        Mod.AddContent(Item);

        if (LoadTrappedChest) {
            TrappedChest = new InstancedTrappedChest(this);
            Mod.AddContent(TrappedChest);
        }
    }

    public sealed override void SetStaticDefaults() {
        this.CloneStaticDefaults(TileID.Containers);

        AdjTiles = [TileID.Containers];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(FindEmptyChest, -1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(AfterPlacement_Hook, -1, 0, false);
        TileObjectData.newTile.AnchorInvalidTiles = [TileID.MagicalIceBlock];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

        if (Settings.MapColor != default) {
            AddMapEntry(Settings.MapColor, Settings.DisplayName?.Value, MapChestName);
        }

        SafeSetStaticDefaults();

        FrameWidth = TileObjectData.newTile.CoordinateFullWidth;
        FrameHeight = TileObjectData.newTile.CoordinateFullHeight;
        TileObjectData.addTile(Type);
    }
    public virtual void SafeSetStaticDefaults() { }

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
    #endregion

    public override LocalizedText DefaultContainerName(int frameX, int frameY) {
        return (Settings.DisplayName?.Value) ?? Lang.GetItemName(Item.Type);
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

    public override bool RightClick(int i, int j) {
        Player player = Main.LocalPlayer;
        Main.mouseRightRelease = false;
        Tile tile = Main.tile[i, j];
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
                if (CanUnlockChest(i, j, chestX, chestY, player) && Chest.Unlock(chestX, chestY)) {
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
}

[Autoload(false)]
internal class InstancedTrappedChest : InstancedModTile, IAddRecipes {
    private readonly UnifiedModChest Parent;

    public readonly ModItem Item;

    public InstancedTrappedChest(UnifiedModChest parent) : base(parent.Name + "Trapped", parent.Texture) {
        Parent = parent;
        TileItemSettings settings = Parent.ItemSettings;
        if (settings.DisplayName?.Value != null) {
            settings = settings.AClone();
            settings.DisplayName = new(Language.GetOrRegister($"{settings.DisplayName!.Value.Key}_Trapped", () => $"Trapped {settings.DisplayName.Value}"));
        }
        Item = new InstancedTileItem(this /*, journeyOverride: new JourneySortByTileId(TileID.FakeContainers2)*/, Settings: settings);
    }

    public override string LocalizationCategory => Parent.LocalizationCategory;

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.TrapSigned[Item.Type] = true;

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
        TileObjectData.newTile.CoordinateHeights = [
            16,
            18
        ];
        TileObjectData.newTile.AnchorInvalidTiles = [
            TileID.MagicalIceBlock
        ];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.addTile(Type);

        if (Parent.Settings.MapColor != default) {
            AddMapEntry(Parent.Settings.MapColor, Parent.Settings.DisplayName?.Value);
        }
        //_baseChest.SafeSetStaticDefaults();

        DustType = -1;
        AdjTiles = [
            TileID.FakeContainers,
            TileID.FakeContainers2,
        ];
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override void MouseOver(int i, int j) {
        Player localPlayer = Main.LocalPlayer;
        localPlayer.noThrow = 2;
        localPlayer.cursorItemIconEnabled = true;
        localPlayer.cursorItemIconText = "";
        localPlayer.cursorItemIconID = Parent.Item.Type;
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

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        Parent.PostDraw(i, j, spriteBatch);
    }

    public void AddRecipes() {
        Recipe.Create(Item.Type)
            .AddIngredient(Parent.Type)
            .AddIngredient(ItemID.Wire, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

public class ChestSettings {
    public Color MapColor;
    public Ref<LocalizedText>? DisplayName;
}