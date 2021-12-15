using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GaleStreams;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Content.WorldEvents.ProgressBars;
using AQMod.NPCs.Monsters;
using AQMod.NPCs.Monsters.GaleStreams;
using AQMod.NPCs.Town;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class NPCSpawnManager : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            EventProgressBarManager.PlayerSafe = false;
            try
            {
                if (AQMod.ShouldRemoveSpawns())
                {
                    spawnRate += 10000;
                    maxSpawns = 0;
                    EventProgressBarManager.PlayerSafe = true;
                    return;
                }
                int balloonMerchant = BalloonMerchant.Find();
                if (balloonMerchant != -1)
                {
                    if ((Main.npc[balloonMerchant].Center - player.Center).Length() < 800f)
                    {
                        if (player.CountBuffs() < Player.MaxBuffs)
                        {
                            player.AddBuff(BuffID.PeaceCandle, 1, quiet: true);
                        }
                        spawnRate += 10000;
                        maxSpawns = 0;
                        EventProgressBarManager.PlayerSafe = true;
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
                    if (player.position.Y < AQMod.SpaceLayer - 40 * 16f)
                    {
                        if (GaleStreams.MeteorTime() || GaleStreams.IsActive)
                        {
                            spawnRate /= 2;
                            maxSpawns *= 2;
                        }
                    }
                }
                if (NPC.AnyNPCs(ModContent.NPCType<RedSprite>()))
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
                EventProgressBarManager.PlayerSafe_GaleStreams = false;
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
                if (GlimmerEvent.SpawnsActive(spawnInfo.player))
                {
                    int tileDistance = GlimmerEvent.GetTileDistance(spawnInfo.player);
                    if (tileDistance < 30)
                    {
                        pool.Clear();
                        return;
                    }
                    else if (tileDistance < GlimmerEvent.MaxDistance)
                    {
                        if (tileDistance > GlimmerEvent.HyperStariteDistance) // shouldn't divide by 0...
                        {
                            DecreaseSpawns(1f - 1f / (tileDistance - GlimmerEvent.HyperStariteDistance));
                        }
                        else
                        {
                            DecreaseSpawns(0f);
                        }
                        int layerIndex = GlimmerEvent.GetLayerIndex(tileDistance);
                        if (layerIndex != -1)
                        {
                            for (int i = layerIndex - 1; i >= 0; i--)
                            {
                                pool.Add(GlimmerEvent.Layers[i].NPCType, GlimmerEvent.Layers[i].SpawnChance);
                            }
                            if (layerIndex == GlimmerEvent.Layers.Count - 1)
                            {
                                pool.Add(GlimmerEvent.Layers[layerIndex].NPCType, AQUtils.GetGrad(0, GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.Layers[layerIndex].SpawnChance);
                            }
                            else
                            {
                                pool.Add(GlimmerEvent.Layers[layerIndex].NPCType, 1f - AQUtils.GetGrad(GlimmerEvent.Layers[layerIndex + 1].Distance, GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.Layers[layerIndex].SpawnChance);
                            }
                        }
                    }
                }
                if (spawnInfo.spawnTileY < AQMod.SpaceLayerTile - 40)
                {
                    if (GaleStreams.MeteorTime())
                    {
                        DecreaseSpawns(0.9f);
                        pool.Add(ModContent.NPCType<Meteor>(), 2f);
                    }
                }
                if (GaleStreams.EventActive(spawnInfo.player) && !spawnInfo.playerSafe)
                {
                    EventProgressBarManager.PlayerSafe_GaleStreams = true;
                    float spawnMult = 0.9f;
                    if (AQMod.SudoHardmode)
                    {
                        if (Main.windSpeed > 0.6f)
                        {
                            if (!NPC.AnyNPCs(ModContent.NPCType<RedSprite>()))
                            {
                                pool.Add(ModContent.NPCType<RedSprite>(), 0.1f);
                            }
                        }
                        spawnMult = 0.1f;
                    }
                    DecreaseSpawns(spawnMult);
                    if (NPC.CountNPCS(ModContent.NPCType<Vraine>()) < 2)
                        pool.Add(ModContent.NPCType<Vraine>(), 1f);
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