using Aequus.UI.EventProgressBars;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI
{
    public sealed class AequusUI : ModSystem
    {
        public class InterfaceLayers
        {
            public const string Inventory = "Vanilla: Inventory";
        }

        public const int LeftInv = 20;

        public static int leftInvOffset;
        public static byte linkClickDelay;
        public static byte specialLeftClickDelay;
        public static byte disableItemLeftClick;
        public static int itemSlotContext;

        public static HashSet<int> ValidOnlineLinkedSlotContext { get; private set; }

        public static int BottomInventory => 260;

        public static byte DisableItemLeftClick { get => disableItemLeftClick; set => disableItemLeftClick = Math.Max(disableItemLeftClick, value); }

        public static bool CanDoLeftClickItemActions => specialLeftClickDelay == 0;

        public const float invBackColorMultipler = 0.785f;
        public static readonly Color invBackColor = new Color(63, 65, 151, 255);
        public static Color InventoryBackColor => invBackColor * invBackColorMultipler;

        public override void Load()
        {
            LoadHooks();
            ValidOnlineLinkedSlotContext = new HashSet<int>()
            {
                ItemSlot.Context.EquipAccessory,
                ItemSlot.Context.ModdedAccessorySlot,
                //ItemSlot.Context.EquipAccessoryVanity,
                ItemSlot.Context.InventoryItem,
                ItemSlot.Context.BankItem,
                ItemSlot.Context.ChestItem,
            };
        }
        private void LoadHooks()
        {
            On.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int += Hook_DisableLeftClick;
            On.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += Hook_UpdateStaticContext;
        }

        private void Hook_DisableLeftClick(On.Terraria.UI.ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (disableItemLeftClick == 0)
            {
                orig(inv, context, slot);
            }
        }

        private void Hook_UpdateStaticContext(On.Terraria.UI.ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
        {
            itemSlotContext = context;
            orig(spriteBatch, inv, context, slot, position, lightColor);
        }

        public override void Unload()
        {
            ValidOnlineLinkedSlotContext?.Clear();
            ValidOnlineLinkedSlotContext = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            leftInvOffset = 0;
            Aequus.InventoryInterface.Update(gameTime);
            Aequus.NPCTalkInterface.Update(gameTime);
            if (Main.mouseItem != null && !Main.mouseItem.IsAir)
            {
                specialLeftClickDelay = Math.Max(specialLeftClickDelay, (byte)20);
            }
            else if (specialLeftClickDelay > 0)
            {
                specialLeftClickDelay--;
            }
            if (linkClickDelay > 0)
            {
                linkClickDelay--;
            }
            if (disableItemLeftClick > 0)
            {
                disableItemLeftClick--;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            leftInvOffset = 0;
            ManageInterfaceLayer(layers, "Vanilla: Inventory", "Aequus: NPC Talk Interface", InterfaceScaleType.UI);
            LegacyInsertInterfaceLayer(layers, "Vanilla: Inventory", "Aequus: Inventory", () =>
            {
                LegacyEventProgressBarLoader.Draw();
                return true;
            });
        }
        private void ManageInterfaceLayer(List<GameInterfaceLayer> layers, string defaultLayer, string layerName, InterfaceScaleType scaleType = InterfaceScaleType.UI)
        {
            int layer = -1;
            if (Aequus.NPCTalkInterface.CurrentState is AequusUIState aequusUIState)
            {
                if (!aequusUIState.ModifyInterfaceLayers(layers, ref scaleType))
                {
                    return;
                }
                layer = aequusUIState.GetLayerIndex(layers);
            }
            if (layer == -1)
            {
                layer = layers.FindIndex((g) => g.Name.Equals(defaultLayer));
            }
            if (layer != -1)
            {
                layers.Insert(layer, new LegacyGameInterfaceLayer(layerName, () =>
                {
                    Aequus.NPCTalkInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                    return true;
                }, scaleType));
            }
        }
        private void LegacyInsertInterfaceLayer(List<GameInterfaceLayer> layers, string name, string yourName, GameInterfaceDrawMethod method, InterfaceScaleType scaleType = InterfaceScaleType.UI)
        {
            int index = layers.FindIndex((l) => l.Name.Equals(name));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(yourName, method, scaleType));
            }
        }

        public static int LeftInventory(bool ignoreCreative = false)
        {
            int left = LeftInv;
            if (!ignoreCreative && Main.LocalPlayer.difficulty == 3 && !Main.CreativeMenu.Blocked)
            {
                left += 48;
            }
            return leftInvOffset + left;
        }

        public static void CloseAllInventoryRelatedUI()
        {
            CloseAllInventoryRelatedUI(Main.LocalPlayer);
        }
        public static void CloseAllInventoryRelatedUI(Player player)
        {
            player.chest = -1;
            player.sign = -1;
            player.SetTalkNPC(-1);
            if (player.whoAmI == Main.myPlayer)
            {
                Main.playerInventory = false;
                Main.recBigList = false;
                Main.CreativeMenu.CloseMenu();
                if (PlayerInput.GrappleAndInteractAreShared)
                {
                    PlayerInput.Triggers.JustPressed.Grapple = false;
                }
                Aequus.NPCTalkInterface.SetState(null);
            }
        }

        public static void HoverItem(Item item, int context = -1)
        {
            Main.hoverItemName = item.Name;
            Main.HoverItem = item.Clone();
            Main.HoverItem.tooltipContext = context;
        }
    }
}