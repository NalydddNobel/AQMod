using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.AnalysisQuests
{
    public class AnalysisSystem : ModSystem
    {
        public static HashSet<int> IgnoreItem { get; private set; }
        public static HashSet<int> IgnoreRarities { get; private set; }
        public static Dictionary<int, TrackedItemRarity> RareTracker { get; private set; }
        public static Dictionary<int, int> RarityItemCounts { get; private set; }
        public static Dictionary<int, List<Item>> RarityToItemList { get; private set; }

        public override void Load()
        {
            IgnoreItem = new HashSet<int>();
            IgnoreRarities = new HashSet<int>()
            {
                ItemRarityID.Quest,
                ItemRarityID.Expert,
                ItemRarityID.Master,
            };
            RareTracker = new Dictionary<int, TrackedItemRarity>();
            InitData();
        }

        public override void AddRecipes()
        {
            RarityItemCounts = new Dictionary<int, int>();
            RarityToItemList = new Dictionary<int, List<Item>>();
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                int rare = ContentSamples.ItemsByType[i].rare;
                //Aequus.Instance.Logger.Debug(ContentSamples.ItemsByType[i].Name + ": " + rare);
                if (ItemID.Sets.Deprecated[i])
                {
                    continue;
                }
                if (!RarityToItemList.ContainsKey(rare))
                {
                    RarityToItemList[rare] = new List<Item>();
                }
                RarityToItemList[rare].Add(ContentSamples.ItemsByType[i]);
                if (IgnoreRarities.Contains(rare))
                {
                    if (RarityItemCounts.TryGetValue(rare, out int val))
                    {
                        RarityItemCounts[rare] = val + 1;
                    }
                    else
                    {
                        RarityItemCounts[rare] = 1;
                    }
                }
            }
            foreach (var r in RarityItemCounts)
            {
                if (r.Value < 8)
                {
                    IgnoreRarities.Add(r.Key);
                }
            }
        }

        public override void Unload()
        {
            RareTracker?.Clear();
            RareTracker = null;
        }

        public override void OnWorldLoad()
        {
            InitData();
        }

        public override void OnWorldUnload()
        {
            InitData();
        }

        public static void InitData()
        {
            RareTracker.Clear();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (RareTracker.Count == 0)
                return;
            tag["TrackedRarities"] = RareTracker.Count;

            int i = 0;
            foreach (var rare in RareTracker)
            {
                AequusHelpers.SaveRarity(tag, $"RarityName{i}", $"Rarity{i}", rare.Key);
                tag[$"Value{i}"] = rare.Value.highestValueObtained;
                i++;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (!tag.TryGet("TrackedRarities", out int val))
                return;
            for (int i = 0; i < val; i++)
            {
                if (AequusHelpers.LoadRarity(tag, $"RarityName{i}", $"Rarity{i}", out int value))
                {
                    RareTracker.Add(value, new TrackedItemRarity() { rare = value, highestValueObtained = tag.Get<int>($"Value{i}"), });
                }
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(RareTracker.Count);
            foreach (var rare in RareTracker)
            {
                writer.Write(rare.Key);
                rare.Value.NetSend(writer);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            RareTracker?.Clear();
            foreach (var rare in RareTracker)
            {
                int rareValue = reader.ReadInt32();
                RareTracker.Add(rareValue, TrackedItemRarity.NetReceive(reader, rareValue));
            }
        }
    }
}