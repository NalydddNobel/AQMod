using Aequus.Buffs;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.BuffPotions
{
    public class MercerTonic : ModItem
    {
        public override void Load()
        {
            Aequus.Hook(typeof(NPCLoader).GetMethod(nameof(NPCLoader.EditSpawnRate), BindingFlags.Public | BindingFlags.Static),
                typeof(MercerTonic).GetMethod(nameof(NPCLoader_EditSpawnRateHook), BindingFlags.NonPublic | BindingFlags.Static));
        }

        private delegate void NPCLoader_EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns);

        private static void NPCLoader_EditSpawnRateHook(NPCLoader_EditSpawnRate orig, Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.HasBuff<TonicSpawnratesBuff>()) // completely ignore modded spawnrate modifiers and vanilla modifiers, and set our own.
            {
                spawnRate = 5;
                maxSpawns = 5;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.buffType = ModContent.BuffType<TonicSpawnratesBuff>();
            Item.buffTime = 300;
            Item.maxStack = 9999;
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.buyPrice(silver: 20);
        }
    }
}