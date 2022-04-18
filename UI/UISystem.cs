using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI
{
    public sealed partial class UISystem : ModSystem
    {
        public static byte linkClickDelay;
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }
        public static HashSet<int> ValidOnlineLinkedSlotContext { get; private set; }

        public override void Load()
        {
            LoadHooks();
            ValidOnlineLinkedSlotContext = new HashSet<int>() 
            {
                ItemSlot.Context.EquipAccessory,
                //ItemSlot.Context.EquipAccessoryVanity,
                ItemSlot.Context.InventoryItem,
                ItemSlot.Context.BankItem,
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
    }
}