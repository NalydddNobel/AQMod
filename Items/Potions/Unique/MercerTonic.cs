using Aequus.Buffs.Misc;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Unique {
    public class MercerTonic : ModItem
    {
        private static object Hook_NPCLoader_EditSpawnRate;

        public override void Load()
        {
            Hook_NPCLoader_EditSpawnRate = Aequus.Detour(typeof(NPCLoader).GetMethod(nameof(NPCLoader.EditSpawnRate), BindingFlags.Public | BindingFlags.Static),
                typeof(MercerTonic).GetMethod(nameof(NPCLoader_EditSpawnRateHook), BindingFlags.NonPublic | BindingFlags.Static));
        }

        private delegate void NPCLoader_EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns);

        private static void NPCLoader_EditSpawnRateHook(NPCLoader_EditSpawnRate orig, Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.HasBuff<TonicSpawnratesBuff>()) // completely ignore modded spawnrate modifiers and vanilla modifiers, and set our own.
            {
                spawnRate = 5;
                maxSpawns = 5;
                player.townNPCs = 0f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && (Main.npc[i].friendly || Main.npc[i].boss || Main.npc[i].IsProbablyACritter()) && !Main.npc[i].townNPC)
                    {
                        maxSpawns += (int)Math.Ceiling(Main.npc[i].npcSlots);
                    }
                }
                maxSpawns = Math.Min(maxSpawns, 20);
            }
            else
            {
                orig(player, ref spawnRate, ref maxSpawns);
            }
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void Unload()
        {
            Hook_NPCLoader_EditSpawnRate = null;
        }

        public override void SetDefaults()
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.buffType = ModContent.BuffType<TonicSpawnratesBuff>();
            Item.buffTime = 300;
            Item.maxStack = Item.CommonMaxStack;
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.buyPrice(silver: 50);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff<TonicSpawnratesBuff>() && !player.HasBuff<TonicSpawnratesDebuff>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.RockLobster)
                .AddIngredient(ItemID.Deathweed)
                .AddIngredient(ItemID.Shiverthorn)
                .AddTile(TileID.Bottles)
                .TryRegisterBefore(ItemID.BattlePotion);
        }
    }
}