using Aequus.Core.Initialization;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Common.Tiles;

public abstract class BaseChest : ModTile {
    private LocalizedText _nameCache;

    public ModItem DropItem { get; private set; }

    protected System.Int32 FrameWidth { get; private set; }
    protected System.Int32 FrameHeight { get; private set; }

    public System.Boolean LoadTrappedChest { get; set; } = true;

    public override void Load() {
        DropItem = new InstancedTileItem(this, value: Item.sellPrice(silver: 1));
        Mod.AddContent(DropItem);

        if (LoadTrappedChest) {
            TrappedChest trappedVariant = new TrappedChest(this);
            Mod.AddContent(trappedVariant);
        }
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

        AdjTiles = new System.Int32[] { TileID.Containers };

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
        TileObjectData.newTile.AnchorInvalidTiles = new System.Int32[] { TileID.MagicalIceBlock };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

        SafeSetStaticDefaults();

        FrameWidth = TileObjectData.newTile.CoordinateFullWidth;
        FrameHeight = TileObjectData.newTile.CoordinateFullHeight;
        TileObjectData.addTile(Type);
    }
    public virtual void SafeSetStaticDefaults() { }

    protected System.String MapChestName(System.String name, System.Int32 i, System.Int32 j) {
        GetTopLeft(i, j, out var left, out var top);

        System.Int32 chest = Chest.FindChest(left, top);
        if (chest < 0) {
            return Language.GetTextValue("LegacyChestType.0");
        }

        if (Main.chest[chest].name == "") {
            return name;
        }

        return name + ": " + Main.chest[chest].name;
    }

    public override LocalizedText DefaultContainerName(System.Int32 frameX, System.Int32 frameY) {
        return _nameCache ??= DropItem.DisplayName;
    }

    public override System.UInt16 GetMapOption(System.Int32 i, System.Int32 j) {
        return (System.UInt16)(Main.tile[i, j].TileFrameX / FrameWidth);
    }

    public override System.Boolean HasSmartInteract(System.Int32 i, System.Int32 j, SmartInteractScanSettings settings) {
        return true;
    }

    public override System.Boolean IsLockedChest(System.Int32 i, System.Int32 j) {
        return Main.tile[i, j].TileFrameX / FrameWidth == 1;
    }

    public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
        num = fail ? 1 : 3;
    }

    public override void KillMultiTile(System.Int32 i, System.Int32 j, System.Int32 frameX, System.Int32 frameY) {
        Chest.DestroyChest(i, j);
    }

    public virtual System.Boolean UnlockChest(System.Int32 i, System.Int32 j, System.Int32 left, System.Int32 top, Player player) {
        return true;
    }

    public override System.Boolean RightClick(System.Int32 i, System.Int32 j) {
        var player = Main.LocalPlayer;
        Main.mouseRightRelease = false;
        GetTopLeft(i, j, out var left, out var top);

        player.CloseSign();
        player.SetTalkNPC(-1);
        Main.npcChatCornerItem = 0;
        Main.npcChatText = "";
        if (Main.editChest) {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.editChest = false;
            Main.npcChatText = System.String.Empty;
        }

        if (player.editedChestName) {
            NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
            player.editedChestName = false;
        }

        var isLocked = Chest.IsLocked(left, top);
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
                if (UnlockChest(i, j, left, top, player) && Chest.Unlock(left, top)) {
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, left, top);
                    }
                }
            }
            else {
                System.Int32 chest = Chest.FindChest(left, top);
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

    public virtual System.Int32 HoverItem(System.Int32 i, System.Int32 j, System.Int32 left, System.Int32 top) {
        return DropItem.Type;
    }

    public override void MouseOver(System.Int32 i, System.Int32 j) {
        var player = Main.LocalPlayer;
        var tile = Main.tile[i, j];
        GetTopLeft(i, j, in tile, out var left, out var top);

        var chest = Chest.FindChest(left, top);
        player.cursorItemIconID = -1;
        if (chest < 0) {
            player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
        }
        else {
            System.String defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
            if (player.cursorItemIconText == defaultName) {
                player.cursorItemIconID = HoverItem(i, j, left, top);
                player.cursorItemIconText = "";
            }
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
    }

    public override void MouseOverFar(System.Int32 i, System.Int32 j) {
        MouseOver(i, j);
        var player = Main.LocalPlayer;
        if (player.cursorItemIconText == "") {
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = 0;
        }
    }

    protected void DrawBasicGlowmask(System.Int32 i, System.Int32 j, SpriteBatch spriteBatch, Texture2D texture, Color color) {
        var tile = Main.tile[i, j];
        if (tile.IsTileInvisible) {
            return;
        }

        try {
            GetTopLeft(i, j, out var left, out var top);
            System.Int32 chest = Chest.FindChest(left, top);
            spriteBatch.Draw(texture, new Vector2(i * 16 - (System.Int32)Main.screenPosition.X, j * 16 - (System.Int32)Main.screenPosition.Y) + TileHelper.DrawOffset, new Rectangle(tile.TileFrameX, 38 * (chest == -1 ? 0 : Main.chest[chest].frame) + tile.TileFrameY, 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        catch {
        }
    }

    protected void GetTopLeft(System.Int32 i, System.Int32 j, out System.Int32 left, out System.Int32 top) {
        var tile = Main.tile[i, j];
        left = i - tile.TileFrameX % FrameWidth / 18;
        top = j - tile.TileFrameY % FrameHeight / 18;
    }
    protected void GetTopLeft(System.Int32 i, System.Int32 j, in Tile tileCache, out System.Int32 left, out System.Int32 top) {
        left = i - tileCache.TileFrameX % FrameWidth / 18;
        top = j - tileCache.TileFrameY % FrameHeight / 18;
    }
}

[Autoload(false)]
internal class TrappedChest : InstancedModTile, IAddRecipes {
    private readonly BaseChest _baseChest;

    private ModItem _item;

    public TrappedChest(BaseChest chest) : base(chest.Name + "Trapped", chest.Texture) {
        _baseChest = chest;
    }

    public override void Load() {
        _item = new InstancedTileItem(this, rarity: _baseChest.DropItem.Item.rare, value: _baseChest.DropItem.Item.value);
        Mod.AddContent(_item);
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.TrapSigned[_item.Type] = true;

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
        TileObjectData.newTile.CoordinateHeights = new System.Int32[] {
            16,
            18
        };
        TileObjectData.newTile.AnchorInvalidTiles = new System.Int32[] {
            TileID.MagicalIceBlock
        };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.addTile(Type);

        _baseChest.SafeSetStaticDefaults();

        DustType = -1;
        AdjTiles = new System.Int32[] {
            TileID.FakeContainers,
            TileID.FakeContainers2,
        };
    }

    public override System.Boolean HasSmartInteract(System.Int32 i, System.Int32 j, SmartInteractScanSettings settings) => true;

    public override System.UInt16 GetMapOption(System.Int32 i, System.Int32 j) {
        return (System.UInt16)((Main.tile[i, j] != null) ? (Main.tile[i, j].TileFrameX / 36) : 0);
    }

    public override void MouseOver(System.Int32 i, System.Int32 j) {
        Player localPlayer = Main.LocalPlayer;
        localPlayer.noThrow = 2;
        localPlayer.cursorItemIconEnabled = true;
        localPlayer.cursorItemIconText = "";
        localPlayer.cursorItemIconID = _baseChest.DropItem.Type;
    }

    public override System.Boolean RightClick(System.Int32 i, System.Int32 j) {
        Tile tile = Main.tile[i, j];
        Main.mouseRightRelease = false;
        System.Int32 num = i;
        System.Int32 num2 = j;
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

    public override void AnimateIndividualTile(System.Int32 type, System.Int32 i, System.Int32 j, ref System.Int32 frameXOffset, ref System.Int32 frameYOffset) {
        Tile tile = Main.tile[i, j];
        System.Int32 num = i;
        System.Int32 num2 = j;
        if (tile.TileFrameX % 36 != 0) {
            num--;
        }
        if (tile.TileFrameY != 0) {
            num2--;
        }
        if (Animation.GetTemporaryFrame(num, num2, out System.Int32 num3)) {
            frameYOffset = 38 * num3;
        }
    }

    public static void Trigger(System.Int32 i, System.Int32 j) {
        Tile tile = Main.tile[i, j];
        System.Int32 num = i;
        System.Int32 num2 = j;
        if (tile.TileFrameX % 36 != 0) {
            num--;
        }
        if (tile.TileFrameY != 0) {
            num2--;
        }
        SoundEngine.PlaySound(SoundID.Mech, new(i * 16, j * 16));
        Wiring.TripWire(num, num2, 2, 2);
    }

    public override void PostDraw(System.Int32 i, System.Int32 j, SpriteBatch spriteBatch) {
        _baseChest.PostDraw(i, j, spriteBatch);
    }

    public void AddRecipes(Aequus aequus) {
        Recipe.Create(_item.Type)
            .AddIngredient(_baseChest.Type)
            .AddIngredient(ItemID.Wire, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}