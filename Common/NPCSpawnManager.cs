using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GaleStreams;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.NPCs.Monsters;
using AQMod.NPCs.Monsters.GaleStreams;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class NPCSpawnManager : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.GetModPlayer<AQPlayer>().bossrush)
            {
                spawnRate *= 10;
                maxSpawns = (int)(maxSpawns * 0.1);
            }
            else if (AQMod.ShouldReduceSpawns())
            {
                spawnRate = 1000;
                maxSpawns = 0;
                return;
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
                    if (GaleStreams.MeteorTime())
                    {
                        spawnRate /= 2;
                        maxSpawns *= 2;
                    }
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
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
            if (AQMod.CosmicEvent.SpawnsActive(spawnInfo.player))
            {
                int tileDistance = AQMod.CosmicEvent.GetTileDistance(spawnInfo.player);
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
            if (GaleStreams.EventActive(spawnInfo.player))
            {
                float spawnMult = 0.9f;
                if (!NPC.AnyNPCs(ModContent.NPCType<Vraine>()))
                    pool.Add(ModContent.NPCType<Vraine>(), 0.8f);
                if (NPC.CountNPCS(ModContent.NPCType<StreamingBalloon>()) < 3)
                    pool.Add(ModContent.NPCType<StreamingBalloon>(), 1f);
                if (AQMod.SudoHardmode)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<RedSprite>()))
                    {
                        pool.Add(ModContent.NPCType<RedSprite>(), 1f);
                        spawnMult = 0.5f;
                    }
                    else
                    {
                        spawnMult = 0.1f;
                    }
                }
                DecreaseSpawns(spawnMult);
            }
        }
    }
}