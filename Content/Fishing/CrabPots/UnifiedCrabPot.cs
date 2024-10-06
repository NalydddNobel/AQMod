using Aequus.Common.Drawing;
using Aequus.Common.Drawing.TileAnimations;
using Aequus.Systems.Backpacks;
using ReLogic.Content;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Content.Fishing.CrabPots;

public abstract class UnifiedCrabPot : ModTile, ITileDrawSystem {
    public const int FramesCount = 4;
    private TileObjectData _tileObjectData;
    private Asset<Texture2D> _backTexture;
    private Asset<Texture2D> _highlightTexture;

    int ITileDrawSystem.Type => Type;

    public sealed override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.AnchorTop = AnchorData.Empty;
        TileObjectData.newTile.AnchorRight = AnchorData.Empty;
        TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TECrabPot>().Hook_AfterPlacement, -1, 0, false);
        SetupCrabPotContent();
        _tileObjectData = TileObjectData.newTile;
        TileObjectData.addTile(Type);
    }

    protected virtual void SetupCrabPotContent() {
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 22 };
        TileObjectData.newTile.DrawYOffset = -12;
    }

    protected virtual bool ValidLiquid(int liquid) {
        return liquid switch {
            LiquidID.Water => _tileObjectData.WaterPlacement != LiquidPlacement.NotAllowed,
            LiquidID.Lava => _tileObjectData.LavaPlacement != LiquidPlacement.NotAllowed,
            _ => false,
        };
    }

    protected bool CanPlaceAt(int i, int j) {
        int waterLevelValid = 2;
        if (!WorldGen.InWorld(i, j, 18 + waterLevelValid)) {
            return false;
        }

        int l = j;
        for (; l > j - waterLevelValid; l--) {
            if (Main.tile[i, l].LiquidAmount == 0 || !ValidLiquid(Main.tile[i, l].LiquidType)) {
                break;
            }
        }

        return Main.tile[i, l - 1].LiquidAmount <= 60;
    }

    public int PlacementPreviewHook_CheckIfCanPlace(int x, int y, int type, int style = 0, int direction = 1, int alternate = 0) {
        for (int k = x; k < x + 2; k++) {
            for (int l = y; l < y + 2; l++) {
                if (!CanPlaceAt(k, l)) {
                    return 1;
                }
            }
        }
        return 0;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = 0;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override void MouseOver(int i, int j) {
        var player = Main.LocalPlayer;
        int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        int top = j - Main.tile[i, j].TileFrameY % 42 / 18;
        int hoverItem = ItemID.None;
        if (TileEntity.ByPosition.TryGetValue(new(left, top), out var tileEntity) && tileEntity is TECrabPot crabPot) {
            if (!crabPot.item.IsAir) {
                hoverItem = crabPot.item.type;
            }
            else {
                var baitItem = GetBaitItem(player, left, top);
                if (baitItem != null) {
                    hoverItem = baitItem.type;
                }
            }
        }

        if (hoverItem != ItemID.None) {
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = hoverItem;
        }
    }

    private static void TryAddBait(int x, int y, int plr, TECrabPot crabPot) {
        if (Main.myPlayer != plr || !crabPot.item.IsAir) {
            return;
        }

        var bait = GetBaitItem(Main.player[plr], x, y);
        if (bait == null) {
            return;
        }

        crabPot.ClearItem();
        crabPot.item = bait.Clone();
        crabPot.item.stack = 1;
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ModContent.GetInstance<PacketCrabPotAddBait>().Send(x, y, Main.myPlayer, crabPot.item);
        }

        bait.stack--;
        if (bait.stack <= 0) {
            bait.TurnToAir();
        }
        SoundEngine.PlaySound(SoundID.Grab, new Vector2(x, y).ToWorldCoordinates());
    }

    private static bool ValidBait(Item item, int liquidType) {
        if (item.IsAir || item.bait <= 0) {
            return false;
        }
        if (liquidType == LiquidID.Lava && !ItemID.Sets.IsLavaBait[item.type]) {
            return false;
        }
        return true;
    }

    private static int GetLiquidType(int x, int y) {
        for (int i = x; i <= x + 2; i++) {
            for (int j = y; j <= y + 2; j++) {
                if (Main.tile[i, j].LiquidAmount > 0) {
                    return Main.tile[i, j].LiquidType;
                }
            }
        }
        return -1;
    }

    public static Item GetBaitItem(Player player, int x, int y) {
        var backpackPlayer = player.GetModPlayer<BackpackPlayer>();
        int liquidType = GetLiquidType(x, y);
        var heldItem = player.HeldItemFixed();
        if (ValidBait(heldItem, liquidType)) {
            return heldItem;
        }

        // Ammo slots
        for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++) {
            if (ValidBait(player.inventory[i], liquidType)) {
                return player.inventory[i];
            }
        }

        // Inventory Slots
        for (int i = Main.InventoryItemSlotsStart; i < Main.InventoryItemSlotsCount; i++) {
            if (ValidBait(player.inventory[i], liquidType)) {
                return player.inventory[i];
            }
        }

        // Backpack slots
        for (int k = 0; k < backpackPlayer.backpacks.Length; k++) {
            if (backpackPlayer.backpacks[k].IsActive(player) && backpackPlayer.backpacks[k].SupportsConsumeItem) {
                var inventory = backpackPlayer.backpacks[k].Inventory;
                for (int i = 0; i < inventory.Length; i++) {
                    if (ValidBait(inventory[i], liquidType)) {
                        return inventory[i];
                    }
                }
            }
        }
        return null;
    }

    public static void GrabItem(int x, int y, int plr, TECrabPot crabPot) {
        if (Main.myPlayer == plr && !crabPot.item.IsAir) {
            Main.player[plr].GiveItem(crabPot.item.Clone(), new EntitySource_TileInteraction(Main.player[plr], x, y), GetItemSettings.LootAllSettingsRegularChest);
        }
    }

    public override bool RightClick(int i, int j) {
        var player = Main.LocalPlayer;
        int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        int top = j - Main.tile[i, j].TileFrameY % 42 / 18;

        if (!TileEntity.ByPosition.TryGetValue(new(left, top), out var tileEntity) || tileEntity is not TECrabPot crabPot) {
            return false;
        }

        if (crabPot.item.IsAir) {
            TryAddBait(left, top, Main.myPlayer, crabPot);
        }
        else {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                ModContent.GetInstance<PacketCrabPotUse>().Send(left, top, Main.myPlayer, TECrabPot.GetWaterStyle(left, top));
                return true;
            }

            GrabItem(left, top, Main.myPlayer, crabPot);
            crabPot.ClearItem();
        }

        AnimationSystem.GetValueOrAddDefault<AnimationOpenCrabPot>(left, top);
        crabPot.biomeData = new(TECrabPot.GetWaterStyle(left, top));
        return true;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        if (WorldGen.destroyObject || CanPlaceAt(i, j)) {
            return true;
        }

        WorldGen.KillTile(i, j);
        return false;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        ModContent.GetInstance<TECrabPot>().Kill(i, j);
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)(Main.tile[i, j].TileFrameX / 36);
    }

    public override void HitWire(int i, int j) {
        int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        int top = j - Main.tile[i, j].TileFrameY % 42 / 18;

        Wiring.SkipWire(left, top);
        Wiring.SkipWire(left + 1, top);
        Wiring.SkipWire(left, top + 1);
        Wiring.SkipWire(left + 1, top + 1);

        if (!TileEntity.ByPosition.TryGetValue(new(left, top), out var tileEntity) || tileEntity is not TECrabPot crabPot || !crabPot.caught) {
            return;
        }

        Item.NewItem(new EntitySource_Wiring(i, j), new Rectangle(left * 16, top * 16, 2, 2), crabPot.item.Clone());
        crabPot.ClearItem();
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        return false;
    }

    private static void DrawCrabPot(int x, int y, SpriteBatch spriteBatch, Texture2D texture, TileObjectData data, int waterYOffset, int animFrame, Func<int, int, Color, Color> blockColor) {
        for (int k = x; k < x + 2; k++) {
            for (int l = y; l < y + 2; l++) {
                if (Main.tile[k, l].IsTileInvisible) {
                    continue;
                }
                var lightColor = Main.tile[k, l].IsTileFullbright ? Color.White : Lighting.GetColor(k, l);
                var drawCoordinates = new Vector2(k, l).ToWorldCoordinates(0f, 0f) + new Vector2(data.DrawXOffset, data.DrawYOffset + waterYOffset) - Main.screenPosition;
                var tileFrame = new Rectangle(Main.tile[k, l].TileFrameX, Main.tile[k, l].TileFrameY + data.CoordinateFullHeight * animFrame, data.CoordinateWidth, data.CoordinateHeights[Main.tile[k, l].TileFrameY % 42 / 18]);
                spriteBatch.Draw(texture, drawCoordinates, tileFrame, blockColor(k, l, lightColor), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }

    private static void DrawItem(int x, int y, SpriteBatch spriteBatch, int waterYOffset, Color lightColor) {
        if (TileEntity.ByPosition.TryGetValue(new(x, y), out var tileEntity) && tileEntity is TECrabPot crabPot) {
            if (!crabPot.item.IsAir) {
                Main.GetItemDrawFrame(crabPot.item.type, out var itemTexture, out var itemFrame);
                float scale = 1f;
                int maxSize = 24;
                int largestSide = Math.Max(itemFrame.Width, itemFrame.Height);
                if (largestSide > maxSize) {
                    scale = maxSize / (float)largestSide;
                }

                spriteBatch.Draw(itemTexture, new Vector2(x, y).ToWorldCoordinates(16f, waterYOffset + 10f) - Main.screenPosition, itemFrame, lightColor, 0f, itemFrame.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
        }
    }

    protected virtual void CustomPreDraw(int i, int j, int waterYOffset, SpriteBatch spriteBatch, TECrabPot crabPot) {
    }

    void DrawAllCrabPots(SpriteBatch sb) {
        foreach (Point p in this.GetDrawPoints()) {
            int i = p.X;
            int j = p.Y;
            Tile tile = Main.tile[p];
            if (!CanPlaceAt(i, j)) {
                WorldGen.KillTile(i, j);
                return;
            }

            _backTexture ??= ModContent.Request<Texture2D>(Texture + "_Back");
            _highlightTexture ??= ModContent.Request<Texture2D>(Texture + "_Highlight");

            var spriteBatch = Main.spriteBatch;
            int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
            int top = j - Main.tile[i, j].TileFrameY % 42 / 18;
            int right = left + 1;
            int bottom = top + 1;

            var data = TileObjectData.GetTileData(Main.tile[i, j]);

            int yOffset = (int)TileHelper.GetWaterY(Main.tile[i, j].LiquidAmount) + (int)(MathF.Sin(Main.GameUpdateCount / 40f) * 2.5f);

            int crabPotAnimation = 0;
            if (AnimationSystem.TryGet<AnimationOpenCrabPot>(new(left, top), out var openAnim)) {
                crabPotAnimation = openAnim.RealFrame;
            }
            if (AnimationSystem.TryGet<AnimationPlaceCrabPot>(new(left, top), out var placeAnim)) {
                yOffset += placeAnim.DrawOffsetY;
            }

            DrawCrabPot(left, top, spriteBatch, _backTexture.Value, data, yOffset, crabPotAnimation, (x, y, rgb) => rgb);
            DrawItem(left, top, spriteBatch, yOffset, Lighting.GetColor(i, j));
            DrawCrabPot(left, top, spriteBatch, TextureAssets.Tile[Type].Value, data, yOffset, crabPotAnimation, (x, y, rgb) => rgb);

            if (Main.InSmartCursorHighlightArea(i, j, out var actuallySelected)) {
                DrawCrabPot(left, top, spriteBatch, _highlightTexture.Value, data, yOffset, crabPotAnimation, (x, y, rgb) => Colors.GetSelectionGlowColor(actuallySelected, (rgb.R + rgb.G + rgb.B) / 3));
            }
            if (TileEntity.ByPosition.TryGetValue(new(left, top), out var tileEntity) && tileEntity is TECrabPot crabPot) {
                CustomPreDraw(left, top, yOffset, spriteBatch, crabPot);
            }
        }
    }

    bool IGridDrawSystem.Accept(Point p) {
        return Main.tile[p].TileFrameX % 36 == 0 && Main.tile[p].TileFrameY % 36 == 0;
    }

    void IDrawSystem.Activate() {
        DrawLayers.Instance.PostDrawMasterRelics += DrawAllCrabPots;
    }

    void IDrawSystem.Deactivate() {
        DrawLayers.Instance.PostDrawMasterRelics -= DrawAllCrabPots;
    }
}