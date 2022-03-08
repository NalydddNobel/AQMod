using AQMod.Common;
using AQMod.Content.World.Events;
using AQMod.NPCs.Bosses;
using AQMod.NPCs.Friendly;
using AQMod.NPCs.Monsters.GaleStreamsMonsters;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public class NPCSpawns : GlobalNPC
    {
        internal static bool SpawnRate_CheckBosses()
        {
            return NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()) || NPC.AnyNPCs(ModContent.NPCType<JerryCrabson>());
        }
        internal static bool SpawnRate_CheckEvents()
        {
            return Glimmer.deactivationDelay > 0;
        }
        internal static bool ShouldRemoveSpawns(Player player)
        {
            return SpawnRate_CheckEvents() || (ModContent.GetInstance<AQConfigServer>().reduceSpawns && player.Biomes().zoneBoss);
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            EventProgressBarLoader.PlayerSafe = false;
            try
            {
                if (ShouldRemoveSpawns(player))
                {
                    spawnRate += 10000;
                    maxSpawns = 0;
                    EventProgressBarLoader.PlayerSafe = true;
                    return;
                }
                int balloonMerchant = BalloonMerchant.Find();
                if (balloonMerchant != -1)
                {
                    if ((Main.npc[balloonMerchant].Center - player.Center).Length() < 800f)
                    {
                        if (player.CountBuffs() < Player.MaxBuffs)
                            player.AddBuff(BuffID.PeaceCandle, 1, quiet: true);
                        spawnRate += 10000;
                        maxSpawns = 0;
                        EventProgressBarLoader.PlayerSafe = true;
                        return;
                    }
                }
                if (player.Biomes().zoneDemonSiege)
                {
                    spawnRate *= 10;
                    maxSpawns = (int)(maxSpawns * 0.1);
                }
                else
                {
                    if (player.position.Y < GaleStreams.MinimumGaleStreamsSpawnOverride) // 160 tiles from the very top of the world
                    {
                        if (GaleStreams.IsActive)
                            spawnRate /= 2;
                        if (GaleStreams.MeteorTime())
                        {
                            spawnRate /= 2;
                            maxSpawns *= 2;
                        }
                    }
                }
                if (NPC.AnyNPCs(ModContent.NPCType<RedSprite>()) || NPC.AnyNPCs(ModContent.NPCType<SpaceSquid>()))
                {
                    spawnRate *= 3;
                    maxSpawns = (int)(maxSpawns * 0.75);
                }
            }
            catch
            {
            }
        }

        private void DecSpawns(IDictionary<int, float> pool, float mult)
        {
            int[] keys = new int[pool.Count];
            int i = 0;
            foreach (var pair in pool)
            {
                keys[i] = pair.Key;
                i++;
            }
            for (int k = 0; k < pool.Count; k++)
            {
                pool[keys[k]] *= mult;
            }

        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            try
            {
                EventProgressBarLoader.ShouldShowGaleStreamsProgressBar = false;
                if (spawnInfo.player.Biomes().zoneCrabCrevice)
                {
                    DecSpawns(pool, 0.05f);
                }
                if (spawnInfo.spawnTileY < 160)
                {
                    if (GaleStreams.MeteorTime())
                    {
                        DecSpawns(pool, 0.9f);
                        pool.Add(ModContent.NPCType<Meteor>(), 2f);
                    }
                }
                if (Glimmer.SpawnsCheck(spawnInfo.player))
                {
                    int tileDistance = Glimmer.Distance(spawnInfo.player);
                    if (tileDistance < 30)
                    {
                        pool.Clear();
                        return;
                    }
                    else if (tileDistance < Glimmer.MaxDistance)
                    {
                        if (tileDistance > Glimmer.HyperStariteDistance) // shouldn't divide by 0...
                            DecSpawns(pool, 1f - 1f / (tileDistance - Glimmer.HyperStariteDistance));
                        else
                            DecSpawns(pool, 0f);
                        int layerIndex = spawnInfo.player.Biomes().zoneGlimmerEventLayer;
                        if (layerIndex != 255)
                        {
                            for (int i = layerIndex - 1; i >= 0; i--)
                            {
                                pool.Add(Glimmer.Layers[i].NPCType, Glimmer.Layers[i].SpawnChance);
                            }
                            if (layerIndex == Glimmer.Layers.Count - 1)
                                pool.Add(Glimmer.Layers[layerIndex].NPCType, AQUtils.GetParabola(0, Glimmer.Layers[layerIndex].Distance, tileDistance) * Glimmer.Layers[layerIndex].SpawnChance);
                            else
                            {
                                pool.Add(Glimmer.Layers[layerIndex].NPCType, 1f - AQUtils.GetParabola(Glimmer.Layers[layerIndex + 1].Distance, Glimmer.Layers[layerIndex].Distance, tileDistance) * Glimmer.Layers[layerIndex].SpawnChance);
                            }
                        }
                    }
                }
                if (GaleStreams.EventActive(spawnInfo.player) && !spawnInfo.playerSafe)
                {
                    EventProgressBarLoader.ShouldShowGaleStreamsProgressBar = true;
                    bool decSpawns = true;
                    if (WorldDefeats.SudoHardmode)
                    {
                        if (Main.windSpeed.Abs() > 0.6f)
                        {
                            bool minibossActive = NPC.AnyNPCs(ModContent.NPCType<RedSprite>()) || NPC.AnyNPCs(ModContent.NPCType<SpaceSquid>());
                            if (!minibossActive)
                            {
                                DecSpawns(pool, MathHelper.Lerp(1f, 0.1f, SpawnCondition.Sky.Chance));
                                decSpawns = false;
                                pool.Add(ModContent.NPCType<RedSprite>(), 0.06f * SpawnCondition.Sky.Chance);
                                pool.Add(ModContent.NPCType<SpaceSquid>(), 0.06f * SpawnCondition.Sky.Chance);
                            }
                        }
                    }
                    if (decSpawns)
                        DecSpawns(pool, MathHelper.Lerp(1f, 0.9f, SpawnCondition.Sky.Chance));
                    if (NPC.CountNPCS(ModContent.NPCType<Vraine>()) < 2)
                        pool.Add(ModContent.NPCType<Vraine>(), 1f * SpawnCondition.Sky.Chance);
                    if (WorldGen.SolidTile(spawnInfo.spawnTileX, spawnInfo.spawnTileY))
                        pool.Add(ModContent.NPCType<StreamingBalloon>(), 0.6f * SpawnCondition.Sky.Chance);
                    pool.Add(ModContent.NPCType<WhiteSlime>(), 0.3f * SpawnCondition.Sky.Chance);
                }
            }
            catch
            {
            }
        }
    }
}