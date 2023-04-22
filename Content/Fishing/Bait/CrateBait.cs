using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Bait
{
    public class CrateBait : ModItem, IModifyFishItem
    {
        public struct BiomeCrateFishingInfo
        {
            public Func<FishingAttempt, Player, bool> BiomeCrateCondition;
            public int ItemID;
            public int HardItemID;

            public BiomeCrateFishingInfo(Func<FishingAttempt, Player, bool> condition, int itemID, int hardmodeItemID)
            {
                BiomeCrateCondition = condition;
                ItemID = itemID;
                HardItemID = hardmodeItemID;
            }
        }

        public static List<BiomeCrateFishingInfo> BiomeCrates { get; private set; }

        public override void Load()
        {
            BiomeCrates = new List<BiomeCrateFishingInfo>()
            {
                new BiomeCrateFishingInfo((a, p) => p.ZoneCorrupt, ItemID.CorruptFishingCrate, ItemID.CorruptFishingCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneCrimson, ItemID.CrimsonFishingCrate, ItemID.CrimsonFishingCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneDungeon, ItemID.DungeonFishingCrate, ItemID.DungeonFishingCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneSkyHeight, ItemID.FloatingIslandFishingCrate, ItemID.FloatingIslandFishingCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneHallow, ItemID.HallowedFishingCrate, ItemID.HallowedFishingCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneJungle, ItemID.JungleFishingCrate, ItemID.JungleFishingCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneSnow, ItemID.FrozenCrate, ItemID.FrozenCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneDesert, ItemID.OasisCrate, ItemID.OasisCrateHard),
                new BiomeCrateFishingInfo((a, p) => a.inLava, ItemID.LavaCrate, ItemID.LavaCrateHard),
                new BiomeCrateFishingInfo((a, p) => p.ZoneBeach, ItemID.OceanCrate, ItemID.OceanCrateHard),
            };
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Amber, 5)
                .AddTile(TileID.Bottles)
                .TryRegisterBefore(ItemID.EnchantedNightcrawler);
        }

        public static void ConvertCrate(Player player, FishingAttempt attempt, ref int itemDrop)
        {
            if (itemDrop == ItemID.WoodenCrate || itemDrop == ItemID.IronCrate || itemDrop == ItemID.WoodenCrateHard || itemDrop == ItemID.IronCrateHard || Main.rand.NextBool(4))
            {
                var l = new List<BiomeCrateFishingInfo>(BiomeCrates);
                while (l.Count > 0)
                {
                    int i = Main.rand.Next(l.Count);
                    if (l[i].BiomeCrateCondition.Invoke(attempt, player))
                    {
                        itemDrop = Main.hardMode ? l[i].HardItemID : l[i].ItemID;
                        return;
                    }
                    l.RemoveAt(i);
                }
            }
        }

        public void ModifyFishItem(Player player, Item fish)
        {
            foreach (var c in BiomeCrates)
            {
                //Main.NewText($"{c.ItemID}:{fish.type} | {c.HardItemID}:{fish.type}");
                if (c.ItemID == fish.type || c.HardItemID == fish.type)
                {
                    Item.stack--;
                    if (Item.stack <= 0)
                    {
                        Item.TurnToAir();
                    }
                    if (Main.myPlayer == player.whoAmI)
                        SoundEngine.PlaySound(SoundID.Grab);
                    return;
                }
            }
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }
    }
}