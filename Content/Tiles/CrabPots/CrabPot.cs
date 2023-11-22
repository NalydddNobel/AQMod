using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Content.Items.Material;
using Aequus.Core;
using Aequus.Core.Graphics.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CrabPots;
public class CrabPot : ModTile, IModifyPlacementPreview {
    public const int CopperPot = 0;
    public const int TinPot = 1;
    public const int StyleCount = 2;

    public const int FramesCount = 4;

    public override void Load() {
        AddItem(CopperPot, ItemID.CopperBar, "Copper");
        AddItem(TinPot, ItemID.TinBar, "Tin");

        void AddItem(int style, int barItem, string name) {
            Mod.AddContent(new InstancedTileItem(this, style: style, nameSuffix: name, rarity: ItemRarityID.Blue, value: Item.sellPrice(silver: 20)).WithRecipe((m) => {
                m.CreateRecipe()
                    .AddIngredient(barItem, 10)
                    .AddIngredient(ItemID.Chain, 3)
                    .AddIngredient<CompressedTrash>()
                    .AddTile(TileID.Anvils)
                    .Register();
            }));
        }
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
        TileObjectData.newTile.DrawYOffset = -12;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 22 };
        TileObjectData.newTile.AnchorTop = AnchorData.Empty;
        TileObjectData.newTile.AnchorRight = AnchorData.Empty;
        TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TECrabPot>().Hook_AfterPlacement, -1, 0, false);
        TileObjectData.addTile(Type);
        DustType = DustID.Iron;
        AddMapEntry(new(105, 186, 181), this.GetLocalization("MapEntryCopper"));
        AddMapEntry(new(152, 186, 188), this.GetLocalization("MapEntryTin"));
    }

    private static bool CanPlaceAt(int i, int j) {
        int waterLevelValid = 2;
        if (!WorldGen.InWorld(i, j, 18 + waterLevelValid)) {
            return false;
        }

        int l = j;
        for (; l > j - waterLevelValid; l--) {
            if (Main.tile[i, l].LiquidAmount == 0) {
                break;
            }
        }

        return Main.tile[i, l - 1].LiquidAmount <= 60;
    }

    public static int PlacementPreviewHook_CheckIfCanPlace(int x, int y, int type, int style = 0, int direction = 1, int alternate = 0) {
        for (int k = x; k < x + 2; k++) {
            for (int l = y; l < y + 2; l++) {
                if (!CanPlaceAt(k, l)) {
                    return 1;
                }
            }
        }
        return 0;
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
                var baitItem = GetBaitItem(player);
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

        var bait = GetBaitItem(Main.player[plr]);
        if (bait == null) {
            return;
        }

        crabPot.item = bait.Clone();
        crabPot.item.stack = 1;
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ModContent.GetInstance<PacketCrabPotAddBait>().Send(x, y, Main.myPlayer, crabPot.item);
        }
        if (bait != null) {
            bait.stack--;
            if (bait.stack <= 0) {
                bait.TurnToAir();
            }
        }
        SoundEngine.PlaySound(SoundID.Grab, new Vector2(x, y).ToWorldCoordinates());
    }

    public static void GrabItem(int x, int y, int plr, TECrabPot crabPot) {
        if (Main.myPlayer == plr && !crabPot.item.IsAir) {
            Main.player[plr].GiveItem(crabPot.item.Clone(), new EntitySource_TileInteraction(Main.player[plr], x, y), GetItemSettings.LootAllSettingsRegularChest);
        }
    }

    private static bool ValidBait(Item item) {
        return !item.IsAir && item.bait > 0;
    }

    public static Item GetBaitItem(Player player) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++) {
            if (ValidBait(player.inventory[i])) {
                return player.inventory[i];
            }
        }
        for (int i = Main.InventoryItemSlotsStart; i < Main.InventoryItemSlotsCount; i++) {
            if (ValidBait(player.inventory[i])) {
                return player.inventory[i];
            }
        }
        for (int k = 0; k < aequusPlayer.backpacks.Length; k++) {
            if (aequusPlayer.backpacks[k].IsActive(player)) {
                var inventory = aequusPlayer.backpacks[k].Inventory;
                for (int i = 0; i < inventory.Length; i++) {
                    if (ValidBait(inventory[i])) {
                        return inventory[i];
                    }
                }
            }
        }
        return null;
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
                ModContent.GetInstance<PacketCrabPotUse>().Send(left, top, Main.myPlayer, LiquidsSystem.WaterStyle);
                return true;
            }

            GrabItem(left, top, Main.myPlayer, crabPot);
            crabPot.item.TurnToAir();
        }

        AnimationSystem.GetValueOrAddDefault<AnimationCrabPot>(left, top);
        crabPot.biomeData = new(TECrabPot.WaterStyle);
        return true;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        if (!WorldGen.destroyObject && !CanPlaceAt(i, j)) {
            WorldGen.KillTile(i, j);
            return false;
        }
        return true;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        ModContent.GetInstance<TECrabPot>().Kill(i, j);
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)(Main.tile[i, j].TileFrameX / 36);
    }

    private static void DrawCrabPot(int x, int y, SpriteBatch spriteBatch, Texture2D texture, TileObjectData data, int waterYOffset, int animFrame, Func<int, int, Color, Color> blockColor) {
        for (int k = x; k < x + 2; k++) {
            for (int l = y; l < y + 2; l++) {
                if (Main.tile[k, l].IsTileInvisible) {
                    continue;
                }
                var lightColor = Main.tile[k, l].IsTileFullbright ? Color.White : Lighting.GetColor(k, l);
                var drawCoordinates = new Vector2(k, l).ToWorldCoordinates(0f, 0f) + new Vector2(data.DrawXOffset, data.DrawYOffset + waterYOffset) + TileHelper.DrawOffset - Main.screenPosition;
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

                spriteBatch.Draw(itemTexture, new Vector2(x, y).ToWorldCoordinates(16f, waterYOffset + 10f) - Main.screenPosition + TileHelper.DrawOffset, itemFrame, lightColor, 0f, itemFrame.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (!CanPlaceAt(i, j)) {
            WorldGen.KillTile(i, j);
            return false;
        }

        int yFrame = Main.tile[i, j].TileFrameY % 42 / 18;
        if (Main.tile[i, j].TileFrameX % 36 / 18 == 0 || yFrame == 0) {
            return false;
        }

        int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        int top = j - yFrame;

        var data = TileObjectData.GetTileData(Main.tile[i, j]);

        int yOffset = (int)TileHelper.GetWaterY(Main.tile[i, j].LiquidAmount) + (int)(MathF.Sin(Main.GameUpdateCount / 40f) * 2.5f);

        int crabPotAnimation = 0;
        if (AnimationSystem.TryGet<AnimationCrabPot>(new(left, top), out var tileAnimation)) {
            crabPotAnimation = tileAnimation.RealFrame;
        }

        DrawCrabPot(left, top, spriteBatch, AequusTextures.CrabPot_Back, data, yOffset, crabPotAnimation, (x, y, rgb) => rgb);
        DrawItem(left, top, spriteBatch, yOffset, Lighting.GetColor(i, j));
        DrawCrabPot(left, top, spriteBatch, TextureAssets.Tile[Type].Value, data, yOffset, crabPotAnimation, (x, y, rgb) => rgb);

        if (Main.InSmartCursorHighlightArea(i, j, out var actuallySelected)) {
            DrawCrabPot(left, top, spriteBatch, AequusTextures.CrabPot_Highlight, data, yOffset, crabPotAnimation, (x, y, rgb) => Colors.GetSelectionGlowColor(actuallySelected, (rgb.R + rgb.G + rgb.B) / 3));
        }
        return false;
    }
}