using AQMod.Assets;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents.Glimmer
{
    public struct GlimmerEventLayer
    {
        /// <summary>
        /// Distance away from the center in tiles
        /// </summary>
        public readonly ushort Distance;
        /// <summary>
        /// Anything below 1 means rarer spawns.
        /// </summary>
        public readonly float SpawnChance;
        /// <summary>
        /// The NPC that spawns in this layer.
        /// </summary>
        public readonly int NPCType;

        public GlimmerEventLayer(int npc, ushort distance, float spawnChance)
        {
            NPCType = npc;
            Distance = distance;
            SpawnChance = spawnChance;
        }

        public GlimmerEventLayer(int npc, ushort distance, float spawnChance, string texturePath)
        {
            NPCType = npc;
            Distance = distance;
            SpawnChance = spawnChance;
        }
    }
}