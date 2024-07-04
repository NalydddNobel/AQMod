using System;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.UI;

namespace AequusRemake.Core.UI;

public partial class UISystem : ModSystem {
    public static int bottomLeftInventoryOffsetX;
    public static byte linkClickDelay;
    public static byte specialLeftClickDelay;
    public static byte disableItemLeftClick;

    public static byte DisableItemLeftClick { get => disableItemLeftClick; set => disableItemLeftClick = Math.Max(disableItemLeftClick, value); }

    public static bool CanDoLeftClickItemActions => specialLeftClickDelay == 0;

    public override void Load() {
        On_ItemSlot.LeftClick_ItemArray_int_int += Hook_DisableLeftClick;
        On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw;
        On_ItemSlot.DrawItemIcon += On_ItemSlot_DrawItemIcon;
    }

    public override void UpdateUI(GameTime gameTime) {
        bottomLeftInventoryOffsetX = 0;
        if (Main.mouseItem != null && !Main.mouseItem.IsAir) {
            specialLeftClickDelay = Math.Max(specialLeftClickDelay, (byte)20);
        }
        else if (specialLeftClickDelay > 0) {
            specialLeftClickDelay--;
        }
        if (linkClickDelay > 0) {
            linkClickDelay--;
        }
        if (disableItemLeftClick > 0) {
            disableItemLeftClick--;
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        bottomLeftInventoryOffsetX = 0;
    }

    public static void CloseAllInventoryRelatedUI() {
        CloseAllInventoryRelatedUI(Main.LocalPlayer);
    }
    public static void CloseAllInventoryRelatedUI(Player player) {
        player.chest = -1;
        player.sign = -1;
        player.SetTalkNPC(-1);
        if (player.whoAmI == Main.myPlayer) {
            Main.playerInventory = false;
            Main.recBigList = false;
            Main.CreativeMenu.CloseMenu();
            if (PlayerInput.GrappleAndInteractAreShared) {
                PlayerInput.Triggers.JustPressed.Grapple = false;
            }
            ModContent.GetInstance<NPCChat>().Interface.SetState(null);
        }
    }

    #region Hooks
    private static float On_ItemSlot_DrawItemIcon(On_ItemSlot.orig_DrawItemIcon orig, Item item, int context, SpriteBatch spriteBatch, Vector2 screenPositionForItemCenter, float scale, float sizeLimit, Color environmentColor) {
        bool isSlotDirty = CurrentSlot.Instance.IsDirty;
        if (isSlotDirty) {
            CurrentSlot.Instance.Update(context, 0, item, screenPositionForItemCenter, environmentColor);
        }

        float value = orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);

        if (isSlotDirty) {
            CurrentSlot.Instance.Dirty();
        }
        return value;
    }

    private static void Hook_DisableLeftClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (disableItemLeftClick == 0) {
            orig(inv, context, slot);
        }
    }

    private static void ItemSlot_Draw(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor) {
        bool slotIsntDirty = CurrentSlot.Instance.Update(context, slot, inv, position, lightColor);

        orig(spriteBatch, inv, context, slot, position, lightColor);

        if (slotIsntDirty) {
            CurrentSlot.Instance.Dirty();
        }
    }
    #endregion
}