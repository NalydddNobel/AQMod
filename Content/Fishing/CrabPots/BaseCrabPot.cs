using Aequus.Common.Backpacks;
using Aequus.Common.Tiles.Components;
using Aequus.Core.Graphics.Animations;
using Aequus.Core.Graphics.Tiles;
using ReLogic.Content;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Content.Fishing.CrabPots;

public abstract class BaseCrabPot : ModTile, ISpecialTileRenderer, IModifyPlacementPreview {
    public const Int32 FramesCount = 4;
    private TileObjectData _tileObjectData;
    private Asset<Texture2D> _backTexture;
    private Asset<Texture2D> _highlightTexture;

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
        TileObjectData.newTile.CoordinateHeights = new Int32[] { 16, 22 };
        TileObjectData.newTile.DrawYOffset = -12;
    }

    protected virtual Boolean ValidLiquid(Int32 liquid) {
        return liquid switch {
            LiquidID.Water => _tileObjectData.WaterPlacement != LiquidPlacement.NotAllowed,
            LiquidID.Lava => _tileObjectData.LavaPlacement != LiquidPlacement.NotAllowed,
            _ => false,
        };
    }

    protected Boolean CanPlaceAt(Int32 i, Int32 j) {
        Int32 waterLevelValid = 2;
        if (!WorldGen.InWorld(i, j, 18 + waterLevelValid)) {
            return false;
        }

        Int32 l = j;
        for (; l > j - waterLevelValid; l--) {
            if (Main.tile[i, l].LiquidAmount == 0 || !ValidLiquid(Main.tile[i, l].LiquidType)) {
                break;
            }
        }

        return Main.tile[i, l - 1].LiquidAmount <= 60;
    }

    public Int32 PlacementPreviewHook_CheckIfCanPlace(Int32 x, Int32 y, Int32 type, Int32 style = 0, Int32 direction = 1, Int32 alternate = 0) {
        for (Int32 k = x; k < x + 2; k++) {
            for (Int32 l = y; l < y + 2; l++) {
                if (!CanPlaceAt(k, l)) {
                    return 1;
                }
            }
        }
        return 0;
    }

    public override void NumDust(Int32 i, Int32 j, Boolean fail, ref Int32 num) => num = 0;

    public override Boolean HasSmartInteract(Int32 i, Int32 j, SmartInteractScanSettings settings) => true;

    public override void MouseOver(Int32 i, Int32 j) {
        var player = Main.LocalPlayer;
        Int32 left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        Int32 top = j - Main.tile[i, j].TileFrameY % 42 / 18;
        Int32 hoverItem = ItemID.None;
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

    private static void TryAddBait(Int32 x, Int32 y, Int32 plr, TECrabPot crabPot) {
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

    private static Boolean ValidBait(Item item, Int32 liquidType) {
        if (item.IsAir || item.bait <= 0) {
            return false;
        }
        if (liquidType == LiquidID.Lava && !ItemID.Sets.IsLavaBait[item.type]) {
            return false;
        }
        return true;
    }

    private static Int32 GetLiquidType(Int32 x, Int32 y) {
        for (Int32 i = x; i <= x + 2; i++) {
            for (Int32 j = y; j <= y + 2; j++) {
                if (Main.tile[i, j].LiquidAmount > 0) {
                    return Main.tile[i, j].LiquidType;
                }
            }
        }
        return -1;
    }

    public static Item GetBaitItem(Player player, Int32 x, Int32 y) {
        var backpackPlayer = player.GetModPlayer<BackpackPlayer>();
        Int32 liquidType = GetLiquidType(x, y);
        var heldItem = player.HeldItemFixed();
        if (ValidBait(heldItem, liquidType)) {
            return heldItem;
        }

        // Ammo slots
        for (Int32 i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++) {
            if (ValidBait(player.inventory[i], liquidType)) {
                return player.inventory[i];
            }
        }

        // Inventory Slots
        for (Int32 i = Main.InventoryItemSlotsStart; i < Main.InventoryItemSlotsCount; i++) {
            if (ValidBait(player.inventory[i], liquidType)) {
                return player.inventory[i];
            }
        }

        // Backpack slots
        for (Int32 k = 0; k < backpackPlayer.backpacks.Length; k++) {
            if (backpackPlayer.backpacks[k].IsActive(player) && backpackPlayer.backpacks[k].SupportsConsumeItem) {
                var inventory = backpackPlayer.backpacks[k].Inventory;
                for (Int32 i = 0; i < inventory.Length; i++) {
                    if (ValidBait(inventory[i], liquidType)) {
                        return inventory[i];
                    }
                }
            }
        }
        return null;
    }

    public static void GrabItem(Int32 x, Int32 y, Int32 plr, TECrabPot crabPot) {
        if (Main.myPlayer == plr && !crabPot.item.IsAir) {
            Main.player[plr].GiveItem(crabPot.item.Clone(), new EntitySource_TileInteraction(Main.player[plr], x, y), GetItemSettings.LootAllSettingsRegularChest);
        }
    }

    public override Boolean RightClick(Int32 i, Int32 j) {
        var player = Main.LocalPlayer;
        Int32 left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        Int32 top = j - Main.tile[i, j].TileFrameY % 42 / 18;

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

    public override Boolean TileFrame(Int32 i, Int32 j, ref Boolean resetFrame, ref Boolean noBreak) {
        if (WorldGen.destroyObject || CanPlaceAt(i, j)) {
            return true;
        }

        WorldGen.KillTile(i, j);
        return false;
    }

    public override void KillMultiTile(Int32 i, Int32 j, Int32 frameX, Int32 frameY) {
        ModContent.GetInstance<TECrabPot>().Kill(i, j);
    }

    public override UInt16 GetMapOption(Int32 i, Int32 j) {
        return (UInt16)(Main.tile[i, j].TileFrameX / 36);
    }

    private static void DrawCrabPot(Int32 x, Int32 y, SpriteBatch spriteBatch, Texture2D texture, TileObjectData data, Int32 waterYOffset, Int32 animFrame, Func<Int32, Int32, Color, Color> blockColor) {
        for (Int32 k = x; k < x + 2; k++) {
            for (Int32 l = y; l < y + 2; l++) {
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

    private static void DrawItem(Int32 x, Int32 y, SpriteBatch spriteBatch, Int32 waterYOffset, Color lightColor) {
        if (TileEntity.ByPosition.TryGetValue(new(x, y), out var tileEntity) && tileEntity is TECrabPot crabPot) {
            if (!crabPot.item.IsAir) {
                Main.GetItemDrawFrame(crabPot.item.type, out var itemTexture, out var itemFrame);
                Single scale = 1f;
                Int32 maxSize = 24;
                Int32 largestSide = Math.Max(itemFrame.Width, itemFrame.Height);
                if (largestSide > maxSize) {
                    scale = maxSize / (Single)largestSide;
                }

                spriteBatch.Draw(itemTexture, new Vector2(x, y).ToWorldCoordinates(16f, waterYOffset + 10f) - Main.screenPosition, itemFrame, lightColor, 0f, itemFrame.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
        }
    }

    protected virtual void CustomPreDraw(Int32 i, Int32 j, Int32 waterYOffset, SpriteBatch spriteBatch, TECrabPot crabPot) {
    }

    public override Boolean PreDraw(Int32 i, Int32 j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX % 36 / 18 == 0 && Main.tile[i, j].TileFrameY % 42 / 18 == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PreDrawMasterRelics);
        }
        return false;
    }

    public void Render(Int32 i, Int32 j, Byte layer) {
        if (!CanPlaceAt(i, j)) {
            WorldGen.KillTile(i, j);
            return;
        }

        _backTexture ??= ModContent.Request<Texture2D>(Texture + "_Back");
        _highlightTexture ??= ModContent.Request<Texture2D>(Texture + "_Highlight");

        var spriteBatch = Main.spriteBatch;
        Int32 left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        Int32 top = j - Main.tile[i, j].TileFrameY % 42 / 18;
        Int32 right = left + 1;
        Int32 bottom = top + 1;

        var data = TileObjectData.GetTileData(Main.tile[i, j]);

        Int32 yOffset = (Int32)TileHelper.GetWaterY(Main.tile[i, j].LiquidAmount) + (Int32)(MathF.Sin(Main.GameUpdateCount / 40f) * 2.5f);

        Int32 crabPotAnimation = 0;
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