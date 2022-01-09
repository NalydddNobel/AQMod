using AQMod.Content.World.Events.GaleStreams;
using AQMod.Content.World.Events.ProgressBars;
using AQMod.NPCs.Friendly;
using AQMod.NPCs.Monsters.GaleStreams;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Common;
using AQMod.Content.World.Events.DemonSiege;

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
                if (GlimmerEvent.AreStariteSpawnsCurrentlyActive(spawnInfo.player))
                {
                    int tileDistance = GlimmerEvent.GetTileDistanceUsingPlayer(spawnInfo.player);
                    if (tileDistance < 30)
                    {
                        pool.Clear();
                        return;
                    }
                    else if (tileDistance < GlimmerEvent.MaxDistance)
                    {
                        if (tileDistance > GlimmerEvent.HyperStariteDistance) // shouldn't divide by 0...
                            DecreaseSpawns(1f - 1f / (tileDistance - GlimmerEvent.HyperStariteDistance));
                        else
                        {
                            DecreaseSpawns(0f);
                        }
                        int layerIndex = GlimmerEvent.GetLayerIndexThroughTileDistance(tileDistance);
                        if (layerIndex != -1)
                        {
                            for (int i = layerIndex - 1; i >= 0; i--)
                            {
                                pool.Add(GlimmerEvent.Layers[i].NPCType, GlimmerEvent.Layers[i].SpawnChance);
                            }
                            if (layerIndex == GlimmerEvent.Layers.Count - 1)
                                pool.Add(GlimmerEvent.Layers[layerIndex].NPCType, AQUtils.GetParabola(0, GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.Layers[layerIndex].SpawnChance);
                            else
                            {
                                pool.Add(GlimmerEvent.Layers[layerIndex].NPCType, 1f - AQUtils.GetParabola(GlimmerEvent.Layers[layerIndex + 1].Distance, GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.Layers[layerIndex].SpawnChance);
                            }
                        }
                    }
                }
                if (spawnInfo.spawnTileY < 160)
                {
                    if (GaleStreams.MeteorTime())
                    {
                        DecreaseSpawns(0.9f);
                        pool.Add(ModContent.NPCType<Meteor>(), 2f);
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
                                DecreaseSpawns(0.1f);
                                decSpawns = false;
                                pool.Add(ModContent.NPCType<RedSprite>(), 0.06f);
                                pool.Add(ModContent.NPCType<SpaceSquid>(), 0.06f);
                            }
                        }
                    }
                    if (decSpawns)
                        DecreaseSpawns(0.9f);
                    if (NPC.CountNPCS(ModContent.NPCType<Vraine>()) < 2)
                        pool.Add(ModContent.NPCType<Vraine>(), 1f);
                    if (WorldGen.SolidTile(spawnInfo.spawnTileX, spawnInfo.spawnTileY))
                        pool.Add(ModContent.NPCType<StreamingBalloon>(), 0.6f);
                    pool.Add(ModContent.NPCType<WhiteSlime>(), 0.3f);
                }
            }
            catch
            {
            }
        }
    }
}