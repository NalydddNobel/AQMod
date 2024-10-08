using Aequus.Common.GUI;
using Aequus.Common.UI;
using System;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.UI;

namespace Aequus.Common.Entities.Containers;

public class StorageInterface : NewUILayer, ILoadable {
    public StorageInterface() : base("StorageInterface", AequusUI.InterfaceLayers.Inventory_28, InterfaceScaleType.UI) {
    }

    protected override void OnActivate() {
        Main.playerInventory = true;
        Main.npcChatText = "";
        SoundEngine.PlaySound(SoundID.MenuOpen);
    }

    protected override void OnDeactivate() {
        Main.trashSlotOffset = new Point16(0, 0);
        Main.LocalPlayer.GetModPlayer<StorageItemPlayer>().SetStorage(null);
        SoundEngine.PlaySound(SoundID.MenuClose);
    }

    public override bool OnUIUpdate(GameTime gameTime) {
        var player = Main.LocalPlayer;
        if (!player.TryGetModPlayer(out StorageItemPlayer storagePlayer) || storagePlayer.Storage == null) {
            return false;
        }

        return Main.playerInventory;
    }

    private static Vector2 GetSlotPosition(int i) {
        return new Vector2(74f + 42f * (i % 10), 260f + 42f * (i / 10));
    }

    protected override bool DrawSelf() {
        var player = Main.LocalPlayer;
        if (!player.TryGetModPlayer(out StorageItemPlayer storagePlayer) || storagePlayer.Storage == null) {
            return true;
        }

        Main.inventoryScale = 0.75f;

        SpriteBatch sb = Main.spriteBatch;
        ICustomStorage storage = storagePlayer.Storage;
        Texture2D slotTexture = storage.InventoryBackTexture.Value;
        Vector2 slotOrigin = slotTexture.Size() / 2f;
        float slotWidth = slotTexture.Width * Main.inventoryScale;
        float slotHeight = slotTexture.Width * Main.inventoryScale;
        Color slotColor = Main.inventoryBack;
        int context = ItemSlot.Context.ChestItem;

        int i = -1;
        float farthestPosition = 0f;
        // TODO -- Reduce allocs here.
        foreach (Item item in storage.Items.ToArray()) {
            i++;
            var slotPosition = GetSlotPosition(i);
            farthestPosition = Math.Max(farthestPosition, slotPosition.Y + slotHeight);

            sb.Draw(slotTexture, slotPosition + slotOrigin * Main.inventoryScale, null, slotColor, 0f, slotOrigin, Main.inventoryScale, SpriteEffects.None, 0f);

            if (Main.mouseX >= slotPosition.X && Main.mouseX <= slotPosition.X + slotWidth && Main.mouseY >= slotPosition.Y && Main.mouseY <= slotPosition.Y + slotHeight && !PlayerInput.IgnoreMouseInterface) {
                player.mouseInterface = true;

                HandleSlot(item, player, storage, i);

                if (item != null && !item.IsAir) {
                    AequusUI.HoverItem(item, context);
                }
            }

            if (item == null || item.IsAir) {
                Main.GetItemDrawFrame(ItemID.GoldenKey, out Texture2D goldenKeyTexture, out Rectangle goldenKeyFrame);
                sb.End();
                sb.Begin_UI(immediate: true);
                Helper.ShaderColorOnly.Apply();
                sb.Draw(goldenKeyTexture, slotPosition + slotOrigin * Main.inventoryScale, goldenKeyFrame, slotColor.MultiplyRGBA(Color.Black with { A = 100 }), 0f, goldenKeyFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
                sb.End();
                sb.Begin_UI();
                continue;
            }

            ItemSlotDrawHelper.DrawFullItem(item, context, i, sb, slotPosition, slotPosition + slotOrigin * Main.inventoryScale, Main.inventoryScale, 32f, Color.White, Color.White);
        }

        Main.trashSlotOffset = new Point16(4, Math.Max((int)farthestPosition - 258, 0));

        return true;
    }

    static void HandleSlot(Item item, Player player, ICustomStorage storage, int i) {
        if (player.itemAnimation > 0) {
            return;
        }

        bool shift = ItemSlot.ShiftInUse;

        if (shift && item != null && !item.IsAir) {
            if (player.ItemSpace(item).CanTakeItem) {
                Main.cursorOverride = CursorOverrideID.ChestToInventory;
            }
            else {
                shift = false;
            }
        }

        if (Main.mouseLeftRelease && Main.mouseLeft) {
            bool anyTransfer = false;

            if (shift && item != null && !item.IsAir) {
                player.GetItem(Main.myPlayer, item.Clone(), GetItemSettings.InventoryEntityToPlayerInventorySettings);
                item.TurnToAir();
                anyTransfer = true;
            }
            else if (item != null && !Main.mouseItem.IsAir && Main.mouseItem.type == item.type && ItemLoader.TryStackItems(item, Main.mouseItem, out _) && Main.mouseItem.stack <= 0) {
                anyTransfer = true;
            }
            else if (Main.mouseItem.IsAir) {
                Main.mouseItem = item?.Clone() ?? new Item();
                item?.TurnToAir();
                anyTransfer = true;
            }
            else if (storage.CanTransferItems(player, Main.mouseItem, item, i)) {
                storage.TransferItems(player, Main.mouseItem, item, i);
                SoundEngine.PlaySound(SoundID.Grab);
                Recipe.FindRecipes();
                anyTransfer = true;
            }

            if (anyTransfer) {
                SoundEngine.PlaySound(SoundID.Grab);
                Recipe.FindRecipes();
                return;
            }
        }
        else if (Main.mouseRight && Main.stackSplit <= 1 && item != null) {
            int stackAmount = Main.superFastStack + 1;
            for (int k = 0; k < stackAmount; k++) {
                if (Main.mouseItem.IsAir || (Main.mouseItem.type == item.type && ItemLoader.CanStack(Main.mouseItem, item) && (Main.mouseItem.stack < Main.mouseItem.maxStack))) {
                    if (Main.mouseItem.IsAir) {
                        Main.mouseItem = item.Clone();
                        Main.mouseItem.stack = 1;
                        item.stack--;
                    }
                    else {
                        Main.mouseItem.stack++;
                        item.stack--;
                        item.favorited = false;
                    }

                    if (item.stack <= 0) {
                        item.TurnToAir();
                    }

                    SoundEngine.PlaySound(SoundID.Grab);
                    Recipe.FindRecipes();

                    ItemSlot.RefreshStackSplitCooldown();
                }
            }
        }
    }
}