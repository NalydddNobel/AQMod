using Aequu2.Core.IO;
using Aequu2.DataSets;
using Aequu2.DataSets.Structures;
using Aequu2.NPCs.Town.PhysicistNPC.Analysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequu2.Old.Content.TownNPCs.PhysicistNPC.Analysis;

public class AnalysisSystem : ModSystem {
    public const string SAVEKEY_RARITY_NAME = "r{0}";
    public const string SAVEKEY_HIGHEST_RARITY_PRICE = "v{0}";

    public static HashSet<int> IgnoreRarities { get; private set; } = new();

    public static readonly Dictionary<int, TrackedItemRarity> RareTracker = new();
    public static readonly Dictionary<int, int> RarityItemCounts = new();
    public static readonly Dictionary<int, List<Item>> RarityToItemList = new();

    public static List<IDEntry<ItemID>> PhysicistPrimaryRewardItems { get; private set; } = new();
    public static List<IDEntry<ItemID>> PhysicistSecondaryRewardItems { get; private set; } = new();

    public override void Load() {
        IgnoreRarities.Add(ItemRarityID.Quest);
        IgnoreRarities.Add(ItemRarityID.Expert);
        IgnoreRarities.Add(ItemRarityID.Master);
    }

    public override void AddRecipes() {
        // Mark all of the items in the Physicist's shop as untradeable.
        NPCShopDatabase.TryGetNPCShop(NPCShopDatabase.GetShopName(ModContent.NPCType<Physicist>()), out var shop);
        foreach (var item in shop.ActiveEntries) {
            ItemDataSet.CannotTradeWithPhysicist.Add(item.Item.type);
        }

        // Automatically blacklist rarities which have less than 8 items.
        for (int i = 0; i < ItemLoader.ItemCount; i++) {
            int rare = ContentSamples.ItemsByType[i].rare;

            //Aequu2.Instance.Logger.Debug(ContentSamples.ItemsByType[i].Name + ": " + rare);
            if (ItemSets.Deprecated[i]) {
                continue;
            }

            if (!RarityToItemList.ContainsKey(rare)) {
                RarityToItemList[rare] = new List<Item>();
            }
            RarityToItemList[rare].Add(ContentSamples.ItemsByType[i]);

            if (!IgnoreRarities.Contains(rare)) {
                if (RarityItemCounts.TryGetValue(rare, out int val)) {
                    RarityItemCounts[rare] = val + 1;
                }
                else {
                    RarityItemCounts[rare] = 1;
                }
            }
        }
        foreach (var r in RarityItemCounts) {
            if (r.Value < 8) {
                IgnoreRarities.Add(r.Key);
            }
        }
    }

    public override void Unload() {
        IgnoreRarities.Clear();
        RareTracker.Clear();
        RarityToItemList.Clear();
        RarityItemCounts.Clear();
    }

    public override void OnWorldLoad() {
        ClearRarityTracker();
    }

    public override void OnWorldUnload() {
        ClearRarityTracker();
    }

    public static void ClearRarityTracker() {
        RareTracker.Clear();
    }

    public override void SaveWorldData(TagCompound tag) {
        if (RareTracker.Count == 0) {
            return;
        }

        tag["TrackedRarities"] = RareTracker.Count;

        int identifier = 0;
        foreach (var rare in RareTracker) {
            IDCommons.SaveRarityToTag(tag, string.Format(SAVEKEY_RARITY_NAME, identifier), rare.Key);
            tag[string.Format(SAVEKEY_HIGHEST_RARITY_PRICE, identifier)] = rare.Value.highestValueObtained;
            identifier++;
        }
    }

    public override void LoadWorldData(TagCompound tag) {
        if (!tag.TryGet("TrackedRarities", out int val)) {
            return;
        }

        for (int i = 0; i < val; i++) {
            if (IDCommons.LoadRarityFromTag(tag, string.Format(SAVEKEY_RARITY_NAME, i), out int value)) {
                RareTracker.Add(value, new TrackedItemRarity() {
                    rare = value,
                    highestValueObtained = tag.Get<int>(string.Format(SAVEKEY_HIGHEST_RARITY_PRICE, i)),
                });
            }
        }
    }

    // Rarity database is server-side only...

    public override void NetSend(BinaryWriter writer) {
        //writer.Write(RareTracker.Count);
        //foreach (var rare in RareTracker)
        //{
        //    writer.Write(rare.Key);
        //    rare.Value.NetSend(writer);
        //}
    }

    public override void NetReceive(BinaryReader reader) {
        //int count = reader.ReadInt32();
        //RareTracker?.Clear();
        //foreach (var rare in RareTracker)
        //{
        //    int rareValue = reader.ReadInt32();
        //    RareTracker.Add(rareValue, TrackedItemRarity.NetReceive(reader, rareValue));
        //}
    }

    public static void HandleItemPickup(Player player, Item item) {
        int rarity = item.OriginalRarity;
        int value = ContentSamples.ItemsByType[item.type].value;
        HandleItemPickup(rarity, value);
    }

    public static void HandleItemPickup(int rarity, int value) {
        TrackedItemRarity trackedRarity = CollectionsMarshal.GetValueRefOrAddDefault(RareTracker, rarity, out _) ??= new() { rare = rarity, };
        trackedRarity.highestValueObtained = Math.Max(value, trackedRarity.highestValueObtained);
    }

    public static void AddToTime(double time, double add, bool dayTime, out double result, out bool resultDayTime) {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        while (add > 0) {
            double max = dayTime ? Main.dayLength : Main.nightLength;
            if (time + add > max) {
                add -= (max - time);
                dayTime = !dayTime;
            }
            else {
                time += add;
                add = 0;
            }
            if (stopWatch.ElapsedMilliseconds > 10) {
                break;
            }
        }
        stopWatch.Stop();
        result = time;
        resultDayTime = dayTime;
    }

    public static LocalizedText GetRarityName(int rarity) {
        if (rarity < ItemRarityID.Count) {
            return Language.GetText($"Mods.Aequu2.Misc.Rarity.{rarity}.DisplayName");
        }

        ModRarity modRarity = RarityLoader.GetRarity(rarity);
        if (XLanguage.TryGet($"Mods.Aequu2.Misc.Rarity.{modRarity.Mod.Name}.{modRarity.Name}.DisplayName", out LocalizedText modRarityName)) {
            return modRarityName;
        }

        return Language.GetText(modRarity.PrettyPrintName().Replace("Rarity", "").Trim());
    }
}