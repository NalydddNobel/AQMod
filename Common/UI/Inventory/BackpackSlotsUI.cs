using Aequus.Common.Items.Tooltips;
using Aequus.Common.Players.Backpacks;
using Aequus.Content.Equipment.Accessories.Inventory.ScavengerBag;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.UI.Inventory;

public class BackpackSlotsUI : UILayer {
    public override string Layer => InterfaceLayers.Inventory_28;

    public static string HoveringBackpackSlotName { get; set; }

    public static int SlotWidth = 56;
    public static float InventoryScale = 0.85f;
    public static int BackpackPadding = 6;

    public Vector2 GetSlotOffset(int index) {
        return new Vector2(index / 5 * SlotWidth * Main.inventoryScale, index % 5 * SlotWidth * Main.inventoryScale);
    }

    public override bool Draw(SpriteBatch spriteBatch) {
        HoveringBackpackSlotName = string.Empty;
        if (!Main.playerInventory) {
            return true;
        }
        var player = Main.LocalPlayer;
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return true;
        }

        Main.inventoryScale = InventoryScale;
        var slotPositionOrigin = new Vector2(524f, 42f);
        for (int k = 0; k < aequusPlayer.backpacks.Length; k++) {
            if (aequusPlayer.backpacks[k].Inventory == null) {
                continue;
            }
            DrawBackpack(player, aequusPlayer, aequusPlayer.backpacks[k], ref slotPositionOrigin);
        }
        return true;
    }

    public void DrawBackpack(Player player, AequusPlayer aequusPlayer, BackpackData backpack, ref Vector2 slotPositionOrigin) {
        var slotTexture = AequusTextures.InventoryBack.Value;
        var favoriteTexture = AequusTextures.InventoryBackFavorited;
        var shinyTexture = AequusTextures.InventoryBackNewItem;
        var slotOrigin = slotTexture.Size() / 2f;
        float slotsDrawn = 0f;
        var slotColor = Utils.MultiplyRGBA(backpack.SlotColor, Main.inventoryBack);
        var favoritedSlotColor = Utils.MultiplyRGBA(backpack.FavoritedSlotColor, Main.inventoryBack);
        var newAndShinySlotColor = Utils.MultiplyRGBA(backpack.NewAndShinySlotColor, Main.inventoryBack);
        bool visible = backpack.IsActive(player) && backpack.IsVisible();
        for (; slotsDrawn < backpack.slotsToRender; slotsDrawn++) {
            int i = (int)slotsDrawn;
            if (!backpack.Inventory.IndexInRange(i) || backpack.Inventory[i] == null) {
                continue;
            }

            var position = slotPositionOrigin + GetSlotOffset(i);

            Main.spriteBatch.Draw(slotTexture, position, null, slotColor, 0f, slotOrigin, Main.inventoryScale, SpriteEffects.None, 0f);
            if (backpack.Inventory[i].favorited) {
                Main.spriteBatch.Draw(favoriteTexture, position, null, favoritedSlotColor, 0f, slotOrigin, Main.inventoryScale, SpriteEffects.None, 0f);
            }
            if (backpack.Inventory[i].newAndShiny) {
                Main.spriteBatch.Draw(shinyTexture, position, null, newAndShinySlotColor, 0f, slotOrigin, Main.inventoryScale, SpriteEffects.None, 0f);
            }
            position -= slotOrigin * Main.inventoryScale;

            int context = ItemSlot.Context.BankItem;
            if (Main.mouseX >= position.X && Main.mouseX <= position.X + slotTexture.Width * Main.inventoryScale && Main.mouseY >= position.Y && Main.mouseY <= position.Y + slotTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface) {
                player.mouseInterface = true;
                var invItemOld = backpack.Inventory[i];
                HoveringBackpackSlotName = backpack.GetDisplayName(Main.LocalPlayer);
                ItemSlot.OverrideHover(backpack.Inventory, context, i);
                ItemSlot.LeftClick(backpack.Inventory, context, i);
                ItemSlot.RightClick(backpack.Inventory, context, i);
                if (Main.mouseLeftRelease && Main.mouseLeft) {
                    Recipe.FindRecipes();
                }
                ItemSlot.MouseHover(backpack.Inventory, context, i);
            }
            ItemSlotRenderer.DrawFullItem(backpack.Inventory[i], context, i, Main.spriteBatch, position, position + slotOrigin * Main.inventoryScale, Main.inventoryScale, 32f, Color.White, Color.White);
        }
        if (backpack.nextSlotAnimation > 0f) {
            var position = slotPositionOrigin + GetSlotOffset((int)slotsDrawn);
            slotsDrawn += backpack.nextSlotAnimation;
            float rotation = 0f; /* InventoryUISystem.ExtraInventorySlotAnimation * MathHelper.TwoPi */
            Main.spriteBatch.Draw(slotTexture, position, null, slotColor * backpack.nextSlotAnimation, rotation, slotOrigin, Main.inventoryScale * backpack.nextSlotAnimation, SpriteEffects.None, 0f);
        }
        if (slotsDrawn > 0f) {
            float opacity;
            if (!visible) {
                opacity = Math.Min(InventoryUISystem.CoinsAmmoOffsetX / 50f, 1f);
            }
            else {
                opacity = MathF.Min(slotsDrawn / 2.5f, 1f);
            }// new Vector2(496f, 22f)
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, slotPositionOrigin + new Vector2(-28f, -20f), new(0, 0, 1, 1), slotColor * 0.5f * opacity, 0f, Vector2.Zero, new Vector2(2f, 230f), SpriteEffects.None, 0f);

            var backpackText = backpack.GetDisplayName(player);
            var textOrigin = ChatManager.GetStringSize(FontAssets.MouseText.Value, backpackText, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, backpackText, slotPositionOrigin + new Vector2(-60f + 38f * MathF.Pow(opacity, 2f), textOrigin.Y / 2f - 42f), Main.inventoryBack * opacity, 0f, new Vector2(0f, textOrigin.Y / 2f), new Vector2(1f, opacity));
            float xOffset = backpack.slotCount / 5 * SlotWidth * Main.inventoryScale + BackpackPadding;
            if (!visible) {
                xOffset *= MathF.Pow(slotsDrawn / backpack.slotCount, 2f);
            }
            slotPositionOrigin.X += xOffset;
        }
    }

    public static void AddBackpackWarningTip(Item item) {
        if (!string.IsNullOrEmpty(HoveringBackpackSlotName)) {
            Keyword tooltip = new(HoveringBackpackSlotName, Color.Lerp(Color.SaddleBrown * 1.5f, Color.White, 0.75f), ModContent.ItemType<ScavengerBag>());
            if (item.buffType > 0 && item.buffTime > 0) {
                tooltip.AddLine(Language.GetTextValue("Mods.Aequus.Misc.BagWarningQuickBuff"));
            }
            if (item.ammo > 0 && !item.notAmmo) {
                tooltip.AddLine(Language.GetTextValue("Mods.Aequus.Misc.BagWarningAmmo"));
            }
            if (tooltip.tooltipLines.Count > 0) {
                KeywordSystem.Tooltips.Add(tooltip);
            }
        }
    }
}