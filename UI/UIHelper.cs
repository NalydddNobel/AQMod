using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI
{
    public sealed partial class UIHelper : ModSystem
    {
        public static byte linkClickDelay;
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }
        public static HashSet<int> ValidOnlineLinkedSlotContext { get; private set; }
        public static HashSet<int> ValidModularSlotContext { get; private set; }

        public const int LeftInv = 20;
        public static int leftInvOffset;
        public static int LeftInventory(bool ignoreCreative = false)
        {
            int left = LeftInv;
            if (!ignoreCreative && Main.LocalPlayer.difficulty == 3 && !Main.CreativeMenu.Blocked)
            {
                left += 48;
            }
            return leftInvOffset + left;
        }
        public static int BottomInventory => 280;

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
            ValidModularSlotContext = new HashSet<int>()
            {
                ItemSlot.Context.InventoryItem,
                ItemSlot.Context.EquipArmor,
                ItemSlot.Context.EquipAccessory,
                ItemSlot.Context.BankItem,
                ItemSlot.Context.ChestItem,
                ItemSlot.Context.DisplayDollAccessory,
                ItemSlot.Context.DisplayDollArmor,
                ItemSlot.Context.EquipGrapple,
                ItemSlot.Context.EquipLight,
                ItemSlot.Context.EquipMinecart,
                ItemSlot.Context.EquipMount,
                ItemSlot.Context.ModdedAccessorySlot,
            };
            if (Main.netMode != NetmodeID.Server)
            {
                InventoryInterface = new UserInterface();
                NPCTalkInterface = new UserInterface();
            }
        }

        public override void Unload()
        {
            ValidOnlineLinkedSlotContext?.Clear();
            ValidOnlineLinkedSlotContext = null;
            InventoryInterface = null;
            NPCTalkInterface = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            leftInvOffset = 0;
            InventoryInterface.Update(gameTime);
            NPCTalkInterface.Update(gameTime);
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
            IntoLayer(layers, "Vanilla: Inventory", "Aequus: Inventory", () =>
            {
                InventoryInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                return true;
            });
        }
        private void IntoLayer(List<GameInterfaceLayer> layers, string name, string yourName, GameInterfaceDrawMethod method, InterfaceScaleType scaleType = InterfaceScaleType.UI)
        {
            int index = layers.FindIndex((l) => l.Name.Equals(name));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(yourName, method, scaleType));
            }
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
                NPCTalkInterface.SetState(null);
                InventoryInterface.SetState(null);
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