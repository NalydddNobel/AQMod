using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.UI
{
    partial class UIHelper
    {
        private static byte specialLeftClickDelay;
        private static byte disableItemLeftClick;
        public static byte DisableItemLeftClick { get => disableItemLeftClick; set => disableItemLeftClick = Math.Max(disableItemLeftClick, value); }
        public static bool CanDoLeftClickItemActions => specialLeftClickDelay == 0;

        public static int ItemSlotContext { get; private set; }

        private void LoadHooks()
        {
            On.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_ItemArray_int_int;
            On.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
        }

        private void ItemSlot_LeftClick_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (disableItemLeftClick == 0)
            {
                orig(inv, context, slot);
            }
        }

        private void ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(On.Terraria.UI.ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
        {
            ItemSlotContext = context;
            orig(spriteBatch, inv, context, slot, position, lightColor);
        }
    }
}