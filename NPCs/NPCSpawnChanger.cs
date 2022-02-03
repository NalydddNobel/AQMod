using AQMod.Common;
using AQMod.Content.World;
using AQMod.Content.World.Events;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.NPCs.Bosses;
using AQMod.NPCs.Friendly;
using AQMod.NPCs.Monsters.GaleStreams;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public class NPCSpawnChanger : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            EventProgressBarLoader.PlayerSafe = false;
            try
            {
                if (AQConfigServer.ShouldRemoveSpawns())
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
                if (player.GetModPlayer<AQPlayer>().bossrush)
                {
                    spawnRate *= 10;
                    maxSpawns = (int)(maxSpawns * 0.1);
                }
                if (DemonSiege.CloseEnoughToDemonSiege(player))
                {
                    spawnRate *= 10;
                    maxSpawns = (int)(maxSpawns * 0.1);
                }
                else
                {
                    if (player.position.Y < EventGaleStreams.MinimumGaleStreamsSpawnOverride) // 160 tiles from the very top of the world
                    {
                        if (EventGaleStreams.IsActive)
                            spawnRate /= 2;
                        if (EventGaleStreams.MeteorTime())
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

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            try
            {
                EventProgressBarLoader.ShouldShowGaleStreamsProgressBar = false;
                void DecreaseSpawns(float mult)
                {
                    IEnumerator<int> keys = pool.Keys.GetEnumerator();
                    int[] keyValue = new int[pool.Count];
                    for (int i = 0; i < pool.Count; i++)
                    {
                        keyValue[i] = keys.Current;
                        if (!keys.MoveNext())
                            break;
                    }
                    keys.Dispose();
                    for (int i = 0; i < pool.Count; i++)
                    {
                        pool[keyValue[i]] *= mult;
                    }
                }
                if (spawnInfo.player.Biomes().zoneCrabCrevice)
                {
                    pool[0] *= 0.05f;
                }
                if (EventGlimmer.AreStariteSpawnsCurrentlyActive(spawnInfo.player))
                {
                    int tileDistance = EventGlimmer.GetTileDistanceUsingPlayer(spawnInfo.player);
                    if (tileDistance < 30)
                    {
                        pool.Clear();
                        return;
                    }
                    else if (tileDistance < EventGlimmer.MaxDistance)
                    {
                        if (tileDistance > EventGlimmer.HyperStariteDistance) // shouldn't divide by 0...
                            DecreaseSpawns(1f - 1f / (tileDistance - EventGlimmer.HyperStariteDistance));
                        else
                        {
                            DecreaseSpawns(0f);
                        }
                        int layerIndex = EventGlimmer.GetLayerIndexThroughTileDistance(tileDistance);
                        if (layerIndex != -1)
                        {
                            for (int i = layerIndex - 1; i >= 0; i--)
                            {
                                pool.Add(EventGlimmer.Layers[i].NPCType, EventGlimmer.Layers[i].SpawnChance);
                            }
                            if (layerIndex == EventGlimmer.Layers.Count - 1)
                                pool.Add(EventGlimmer.Layers[layerIndex].NPCType, AQUtils.GetParabola(0, EventGlimmer.Layers[layerIndex].Distance, tileDistance) * EventGlimmer.Layers[layerIndex].SpawnChance);
                            else
                            {
                                pool.Add(EventGlimmer.Layers[layerIndex].NPCType, 1f - AQUtils.GetParabola(EventGlimmer.Layers[layerIndex + 1].Distance, EventGlimmer.Layers[layerIndex].Distance, tileDistance) * EventGlimmer.Layers[layerIndex].SpawnChance);
                            }
                        }
                    }
                }
                if (spawnInfo.spawnTileY < 160)
                {
                    if (EventGaleStreams.MeteorTime())
                    {
                        DecreaseSpawns(0.9f);
                        pool.Add(ModContent.NPCType<Meteor>(), 2f);
                    }
                }
                if (EventGaleStreams.EventActive(spawnInfo.player) && !spawnInfo.playerSafe)
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
                                DecreaseSpawns(MathHelper.Lerp(1f, 0.1f, SpawnCondition.Sky.Chance));
                                decSpawns = false;
                                pool.Add(ModContent.NPCType<RedSprite>(), 0.06f * SpawnCondition.Sky.Chance);
                                pool.Add(ModContent.NPCType<SpaceSquid>(), 0.06f * SpawnCondition.Sky.Chance);
                            }
                        }
                    }
                    if (decSpawns)
                        DecreaseSpawns(MathHelper.Lerp(1f, 0.9f, SpawnCondition.Sky.Chance));
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