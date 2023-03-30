using Aequus.Common;
using Aequus.Common.ModPlayers;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public static HashSet<int> SummonStaff { get; private set; }
        public static HashSet<int> CritOnlyModifier { get; private set; }

        public static List<int> ClassOrderedPillarFragments { get; private set; }
        public static List<int> RainbowOrderPillarFragments { get; private set; }
        public static List<int> FruitIDs { get; private set; }
        public static List<int> LegendaryFishIDs { get; private set; }

        /// <summary>
        /// A list of names for ingame rarities.
        /// </summary>
        public static Dictionary<int, string> RarityNames { get; private set; }

        private static Dictionary<int, int> ItemToBannerCache;

        internal void Load_DataSets()
        {
            FruitIDs = new List<int>()
            {
                ItemID.Apple,
                ItemID.Apricot,
                ItemID.Banana,
                ItemID.BlackCurrant,
                ItemID.BloodOrange,
                ItemID.Cherry,
                ItemID.Coconut,
                ItemID.Dragonfruit,
                ItemID.Elderberry,
                ItemID.Grapefruit,
                ItemID.Lemon,
                ItemID.Mango,
                ItemID.Peach,
                ItemID.Pineapple,
                //ItemID.Pomegranate,
                ItemID.Plum,
                ItemID.Rambutan,
                //ItemID.SpicyPepper,
                ItemID.Starfruit,
            };
            ItemToBannerCache = new Dictionary<int, int>();
            LegendaryFishIDs = new List<int>();
            SummonStaff = new HashSet<int>();
            CritOnlyModifier = new HashSet<int>()
            {
                PrefixID.Keen,
                PrefixID.Zealous,
            };
            RarityNames = new Dictionary<int, string>()
            {
                [ItemRarityID.Master] = "Mods.Aequus.ItemRarity.-13",
                [ItemRarityID.Expert] = "Mods.Aequus.ItemRarity.-12",
                [ItemRarityID.Quest] = "Mods.Aequus.ItemRarity.-11",
                [ItemRarityID.Gray] = "Mods.Aequus.ItemRarity.-1",
                [ItemRarityID.White] = "Mods.Aequus.ItemRarity.0",
                [ItemRarityID.Blue] = "Mods.Aequus.ItemRarity.1",
                [ItemRarityID.Green] = "Mods.Aequus.ItemRarity.2",
                [ItemRarityID.Orange] = "Mods.Aequus.ItemRarity.3",
                [ItemRarityID.LightRed] = "Mods.Aequus.ItemRarity.4",
                [ItemRarityID.Pink] = "Mods.Aequus.ItemRarity.5",
                [ItemRarityID.LightPurple] = "Mods.Aequus.ItemRarity.6",
                [ItemRarityID.Lime] = "Mods.Aequus.ItemRarity.7",
                [ItemRarityID.Yellow] = "Mods.Aequus.ItemRarity.8",
                [ItemRarityID.Cyan] = "Mods.Aequus.ItemRarity.9",
                [ItemRarityID.Red] = "Mods.Aequus.ItemRarity.10",
                [ItemRarityID.Purple] = "Mods.Aequus.ItemRarity.11",
            };
        }

        internal void PostSetupContent_DataSets()
        {
            var contentArray = new ContentArrayFile("ItemSets", ItemID.Search);

            ClassOrderedPillarFragments = contentArray.ReadIntList("ClassOrderedPillarFragments");
            RainbowOrderPillarFragments = contentArray.ReadIntList("RainbowOrderPillarFragments");
        }

        internal void AddRecipes_DataSets()
        {
            if (Aequus.LogMore)
                Aequus.Instance.Logger.Info("Loading rarity name translations...");
            for (int i = ItemRarityID.Purple + 1; i < RarityLoader.RarityCount; i++)
            {
                try
                {
                    var rare = RarityLoader.GetRarity(i);
                    string key = $"Mods.Aequus.ItemRarity.{rare.Mod.Name}.{rare.Name}";
                    if (TextHelper.ContainsKey(key))
                    {
                        RarityNames.Add(rare.Type, key);
                        if (Aequus.LogMore)
                            Aequus.Instance.Logger.Info($"Autoloaded rarity key: {key}");
                    }
                    //else if (Aequus.LogMore)
                    //{
                    //    Aequus.Instance.Logger.Info($"Key not found: {key}");
                    //}
                }
                catch
                {
                }
            }

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = ContentSamples.ItemsByType[i];
                if (item.damage > 0 && item.DamageType == DamageClass.Summon && item.shoot > ProjectileID.None && item.useStyle > ItemUseStyleID.None && (ContentSamples.ProjectilesByType[item.shoot].minionSlots > 0f || ContentSamples.ProjectilesByType[item.shoot].sentry))
                {
                    SummonStaff.Add(i);
                }
            }
        }

        internal void Unload_DataSets()
        {
            ItemToBannerCache?.Clear();
            ItemToBannerCache = null;
            RarityNames?.Clear();
            RarityNames = null;
            LegendaryFishIDs?.Clear();
            LegendaryFishIDs = null;
            SummonStaff?.Clear();
            SummonStaff = null;
            CritOnlyModifier?.Clear();
            CritOnlyModifier = null;
        }
    }
}