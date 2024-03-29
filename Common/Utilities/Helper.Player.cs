﻿using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus {
    public static partial class Helper {
        public static bool PickaxePowerCriteria(this Player player, Item item, int x, int y) {
            var inv = player.inventory;
            player.inventory = new Item[inv.Length];
            for (int i = 0; i < inv.Length; i++) {
                player.inventory[i] = item;
            }
            bool value = false;
            try {
                value = player.HasEnoughPickPowerToHurtTile(x, y);
            }
            catch {
            }
            player.inventory = inv;
            return value;
        }

        public static IEntitySource GetSource_HeldItem(this Player player) {
            return player.GetSource_ItemUse(player.HeldItemFixed());
        }

        public static bool ConsumeItemInInvOrVoidBag(this Player player, int itemType) {
            if (player.ConsumeItem(itemType)) {
                return true;
            }

            if (!player.HasItem(ItemID.VoidLens)) {
                return false;
            }

            for (int i = 0; i < Chest.maxItems; i++) {
                if (!player.bank4.item[i].IsAir && player.bank4.item[i].type == itemType) {
                    if (ItemLoader.ConsumeItem(player.bank4.item[i], player)) {
                        player.bank4.item[i].stack--;
                    }
                    if (player.bank4.item[i].stack <= 0) {
                        player.bank4.item[i].TurnToAir();
                    }
                    return true;
                }
            }
            return false;
        }

        public static Item GetAccessory(this Player player, int slot, bool modded = false) {
            if (modded) {
                var slotInstance = LoaderManager.Get<AccessorySlotLoader>().Get(slot, player);
                return slotInstance?.FunctionalItem ?? new();
            }

            return player.armor[slot];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TotalHearts(this Player player) {
            return Math.Clamp(player.statLifeMax, 1, 20);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HealthPerHeart(this Player player) {
            return player.statLifeMax2 / player.TotalHearts();
        }
    }
}