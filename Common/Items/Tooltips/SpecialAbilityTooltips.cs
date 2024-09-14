using Aequus.Common.Items.EquipmentBooster;
using Aequus.Items.Equipment.Accessories.CrownOfBlood;
using Aequus.Systems.Backpacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Common.Items.Tooltips;

public partial class SpecialAbilityTooltips : GlobalItem {
    private static int _lastHoveredItemID;
    public static readonly List<SpecialAbilityTooltipInfo> Tooltips = [];

    public override void Unload() {
        _lastHoveredItemID = 0;
        Tooltips.Clear();
    }

    private void AddCrownOfBloodTooltip(Item item) {
        SpecialAbilityTooltipInfo tooltip = new(TextHelper.GetItemName<CrownOfBloodItem>().Value, Color.PaleVioletRed, ModContent.ItemType<CrownOfBloodItem>());
        if (item.defense > 0) {
            tooltip.AddLine(TextHelper.GetTextValue("Items.BoostTooltips.Defense", item.defense * 2));
        }

        if (item.wingSlot > -1) {
            tooltip.AddLine(TextHelper.GetTextValue("Items.BoostTooltips.Wings"));
        }

        if (EquipBoostDatabase.Instance.Entries.IndexInRange(item.type) && !EquipBoostDatabase.Instance.Entries[item.type].Invalid) {
            tooltip.AddLine(EquipBoostDatabase.Instance.Entries[item.type].Tooltip.Value);
        }

        if (tooltip.tooltipLines.Count == 0) {
            tooltip.AddLine(TextHelper.GetTextValue("Items.BoostTooltips.UnknownEffect"));
        }
        Tooltips.Add(tooltip);
    }

    private void SetupLinesForItem(Item item) {
        var player = Main.LocalPlayer;
        var aequusPlayer = player.Aequus();
        if (aequusPlayer.accCrownOfBlood != null && item.ModItem is not CrownOfBloodItem && item.accessory && !item.vanity && item.createTile != TileID.MusicBoxes) {
            AddCrownOfBloodTooltip(item);
        }
        if (aequusPlayer.hasBlockGlove && item.createTile >= TileID.Dirt) {
            AddBlockGloveTooltip(item);
        }
        //if (aequusPlayer.accSentryInheritence != null && item.accessory && !item.vanity && item.createTile != TileID.MusicBoxes) {
        //    SpecialAbilityTooltipInfo tooltip = new(aequusPlayer.accSentryInheritence.Name, Color.LawnGreen, aequusPlayer.accSentryInheritence.type);
        //    tooltip.AddLine("Sentries will summon Spores around them to damage enemies");
        //    _tooltips.Add(tooltip);
        //}

        BackpackSlotsUI.AddBackpackWarningTip(item);

        if (item.ModItem is IAddSpecialTooltips addSpecialTooltips) {
            addSpecialTooltips.AddSpecialTooltips(Tooltips);
        }
    }

    public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) {
        // Init lines if hovering over a new item type
        if (_lastHoveredItemID != item.type) {
            Tooltips.Clear(); // Clear line cache
            SetupLinesForItem(item);
            _lastHoveredItemID = item.type;
        }
        // Exit if there are no lines to render, or if the player is holding up (this hopefully prevents conflicts with SLR's keyword system)
        if (Tooltips.Count == 0 || Main.LocalPlayer.controlUp) {
            return true;
        }

        var spriteBatch = Main.spriteBatch;
        var font = FontAssets.MouseText.Value;

        // Recalculate vanilla box width
        int vanillaTooltipBoxWidth = 0;
        for (int i = 0; i < lines.Count; i++) {
            vanillaTooltipBoxWidth = Math.Max(vanillaTooltipBoxWidth, (int)ChatManager.GetStringSize(font, lines[i].Text, Vector2.One).X);
        }

        int lineStartX = x;
        int lineStartY = y;
        int lineDirX = 1;
        int lineDirY = 1;
        int largestBoxWidth = 0;
        int previousBoxHeight = 0;
        for (int i = 0; i < Tooltips.Count; i++) {
            int boxHeight = Tooltips[i].lineTotalHeight + 40;
            if (i > 0) {
                if (lineDirY != -1) {
                    if (lineStartY + previousBoxHeight + boxHeight > Main.screenHeight) {
                        lineDirY = -1;
                        lineStartY = y - boxHeight - 4;
                    }
                    else {
                        lineStartY += previousBoxHeight + 4;
                    }
                }
                else if (lineStartY - boxHeight < 0) {
                    lineDirY = 1;
                    lineStartX += lineDirX * (largestBoxWidth + 24);
                    lineStartY = y;
                }
                else {
                    lineStartY -= boxHeight + 4;
                }
            }
            previousBoxHeight = boxHeight;

            int lineX = lineStartX + vanillaTooltipBoxWidth + 26;
            // Recalculate tooltip box if needed
            if (Tooltips[i].recalculate) {
                Tooltips[i].Recalculate(font);
            }

            // Header values for proper placement
            float headerHalfMeasurementX = ChatManager.GetStringSize(font, Tooltips[i].header, Vector2.One).X / 2f;
            float headerMinX = headerHalfMeasurementX + 6f;

            int boxWidth = Math.Max(Tooltips[i].lineMaxWidth, (int)headerHalfMeasurementX * 2 + 10 + (Tooltips[i].itemIconId > 0 ? 32 : 0));
            largestBoxWidth = Math.Max(boxWidth, largestBoxWidth);
            // Swap box direction to the other side if we're trying to draw outside of the screen
            if (lineX + boxWidth > Main.screenWidth) {
                lineDirX = -1;
            }
            if (lineDirX == -1) {
                lineX = lineStartX - boxWidth - 26;
            }

            // Draw tooltip box
            Helper.DrawUIPanel(spriteBatch, TextureAssets.InventoryBack.Value, new(lineX - 10, lineStartY - 10, boxWidth + 20, Tooltips[i].lineTotalHeight + 40), (Main.inventoryBack * 0.5f) with { A = Main.inventoryBack.A });

            // Draw item icon, if there is one
            int itemIconId = Tooltips[i].itemIconId;
            if (itemIconId > 0) {
                // offset the header's minimum X position
                headerMinX += 32f;

                Main.instance.LoadItem(itemIconId);
                var texture = TextureAssets.Item[itemIconId].Value;
                Helper.GetItemDrawData(itemIconId, out var frame);
                float scale = 1f;
                int largestSide = Math.Max(texture.Width, texture.Height);
                if (largestSide > 32f) {
                    scale = 32f / largestSide;
                }
                spriteBatch.Draw(texture, new Vector2(lineX - 2f, lineStartY - 2f), frame, Main.inventoryBack * 0.8f, 0f, Vector2.Zero, scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            }

            // Draw header
            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                font,
                Tooltips[i].header,
                new Vector2(lineX + Math.Max(boxWidth / 2f, headerMinX), lineStartY),
                Tooltips[i].textColor * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 1.5f, 2f),
                0f,
                new Vector2(headerHalfMeasurementX, 0f),
                Vector2.One
            );

            // Draw lines
            int textLineY = lineStartY + 32;
            for (int j = 0; j < Tooltips[i].tooltipLines.Count; j++) {
                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch,
                    font,
                    Tooltips[i].tooltipLines[j],
                    new Vector2(lineX, textLineY),
                    Tooltips[i].textColor,
                    0f,
                    Vector2.Zero,
                    Vector2.One
                );
                textLineY += Tooltips[i].lineHeights[j];
            }
        }
        return true;
    }
}