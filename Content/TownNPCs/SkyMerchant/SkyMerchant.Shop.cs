using AequusRemake.Content.Configuration;
using AequusRemake.Content.Items.Accessories.Balloons;
using AequusRemake.Content.Items.Accessories.FlashwayShield;
using AequusRemake.Content.Items.Accessories.GoldenFeather;
using AequusRemake.Content.Items.Accessories.Informational.Calendar;
using AequusRemake.Content.Items.Accessories.WeightedHorseshoe;
using AequusRemake.Content.Items.Potions.Healing.Restoration;
using AequusRemake.Content.Items.Tools.Bellows;
using AequusRemake.Content.Items.Tools.NameTag;
using AequusRemake.Content.Items.Weapons.Classless.StunGun;
using AequusRemake.Content.Items.Weapons.Magic.Furystar;
using AequusRemake.Content.Items.Weapons.Ranged.SkyHunterCrossbow;
using AequusRemake.Content.Mounts.HotAirBalloon;
using AequusRemake.Core.Systems;
using System;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Content.TownNPCs.SkyMerchant;

public partial class SkyMerchant {
    public override void AddShops() {
        int celestialMagnetAltId = VanillaChangesConfig.Instance.MoveTreasureMagnet ? ItemID.TreasureMagnet : ItemID.CelestialMagnet;

        new NPCShop(Type, "Shop")
            .AddCustomValue<SkyHunterCrossbow>(Commons.Cost.NPCSkyMerchantCustomPrice * 1.5)
            .AddCustomValue<Bellows>(Commons.Cost.NPCSkyMerchantCustomPrice)
            .AddCustomValue(ModContent.GetInstance<HotAirBalloonMount>().MountItem.Type, Commons.Cost.NPCSkyMerchantCustomPrice * 7)
            .Add<NameTag>()
            .Add<Calendar>()
            .Add<LesserRestorationPotion>()
            .AddCustomValue<SlimyBlueBalloon>(Commons.Cost.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Sunday, DayOfWeek.Monday))
            .AddCustomValue<GoldenFeather>(Commons.Cost.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Monday, DayOfWeek.Tuesday))
            .AddCustomValue(celestialMagnetAltId, Commons.Cost.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Tuesday, DayOfWeek.Wednesday))
            .AddCustomValue<StunGun>(Commons.Cost.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Wednesday, DayOfWeek.Thursday))
            .AddCustomValue<WeightedHorseshoe>(Commons.Cost.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Thursday, DayOfWeek.Friday))
            .AddCustomValue<Furystar>(Commons.Cost.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Friday, DayOfWeek.Saturday))
            .AddCustomValue<FlashwayShield>(Commons.Cost.NPCSkyMerchantCustomPrice, TimeSystem.ConditionByDayOfWeek(DayOfWeek.Saturday))
            .Register();
    }

    public override void ModifyActiveShop(string shopName, Item[] items) {
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

        int nextIndex = ExtendShop.FindNextIndex(items);
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