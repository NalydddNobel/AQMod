﻿using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Items.Equipment.Accessories.Informational.Calendar;
using Aequus.Items.Equipment.Accessories.Movement.FlashwayShield;
using Aequus.Items.Equipment.Accessories.Movement.SlimyBlueBalloon;
using Aequus.Items.Equipment.Accessories.Movement.WeightedHorseshoe;
using Aequus.Items.Equipment.Accessories.Restoration.GoldenFeather;
using Aequus.Items.Weapons.Classless.StunGun;
using Aequus.Items.Weapons.Ranged.Bows.SkyHunterCrossbow;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.TownNPCs.SkyMerchant;

public partial class SkyMerchant {
    public override void AddShops() {
        new NPCShop(Type, "Shop")
            .AddCustomValue<SlimyBlueBalloon>(ItemCommons.Price.SkyMerchantCustomPurchasePrice, AequusConditions.DayOfTheWeek(DayOfWeek.Sunday))
            .AddCustomValue<GoldenFeather>(ItemCommons.Price.SkyMerchantCustomPurchasePrice, AequusConditions.DayOfTheWeek(DayOfWeek.Monday))
            .AddCustomValue<StunGun>(ItemCommons.Price.SkyMerchantCustomPurchasePrice, AequusConditions.DayOfTheWeek(DayOfWeek.Wednesday))
            .AddCustomValue<WeightedHorseshoe>(ItemCommons.Price.SkyMerchantCustomPurchasePrice, AequusConditions.DayOfTheWeek(DayOfWeek.Thursday))
            .AddCustomValue<FlashwayShield>(ItemCommons.Price.SkyMerchantCustomPurchasePrice, AequusConditions.DayOfTheWeek(DayOfWeek.Saturday))
            .AddCustomValue<SkyHunterCrossbow>(ItemCommons.Price.SkyMerchantCustomPurchasePrice * 1.5)
            .Add<Calendar>()
            .Register();
    }

    public override void ModifyActiveShop(string shopName, Item[] items) {
        ModifyActiveShop_TownNPCLoot(shopName, items);
    }

    #region Town NPC loot
    private struct TownNPCDropData {
        public List<DropRateInfo> DropRateInfo;
        public int NPCType;
        public int NPCWhoAmI;

        public TownNPCDropData(NPC npc) {
            NPCType = npc.type;
            NPCWhoAmI = npc.whoAmI;
            DropRateInfo = new();
        }
    }

    public void ModifyActiveShop_TownNPCLoot(string shopName, Item[] items) {
        Dictionary<int, TownNPCDropData> dropRateInfo = new();
        DropRateInfoChainFeed dropRateInfoChainFeed = new(1f);
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].townNPC && !dropRateInfo.ContainsKey(Main.npc[i].type)) {
                var drops = Main.ItemDropsDB.GetRulesForNPCID(Main.npc[i].type, includeGlobalDrops: false);
                if (drops == null) {
                    continue;
                }

                dropRateInfo[Main.npc[i].type] = new(Main.npc[i]);
                foreach (var d in drops) {
                    d.ReportDroprates(dropRateInfo[Main.npc[i].type].DropRateInfo, dropRateInfoChainFeed);
                }
            }
        }

        int nextIndex = items.GetNextIndex();
        foreach (var pair in dropRateInfo) {
            foreach (var dropRateInfoValue in pair.Value.DropRateInfo) {
                if (nextIndex >= items.Length) {
                    return;
                }

                if (CheckConditions(pair.Value, dropRateInfoValue)) {
                    items[nextIndex++] = new(dropRateInfoValue.itemId);
                }
            }
        }

        static bool CheckConditions(TownNPCDropData townNPCDropData, DropRateInfo dropRateInfoValue) {
            if (dropRateInfoValue.conditions != null) {
                DropAttemptInfo dropAttemptInfo = new() {
                    npc = Main.npc[townNPCDropData.NPCWhoAmI],
                    IsExpertMode = Main.expertMode,
                    IsMasterMode = Main.masterMode,
                    player = Main.LocalPlayer,
                    rng = Main.rand,
                    IsInSimulation = true,
                };
                foreach (var condition in dropRateInfoValue.conditions) {
                    if (!condition.CanDrop(dropAttemptInfo)) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
    #endregion
}