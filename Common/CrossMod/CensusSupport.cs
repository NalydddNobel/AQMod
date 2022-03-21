using AQMod.NPCs.Friendly;
using System;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    public sealed class CensusSupport
    {
        private static void AddNPC<T>(string canSpawn) where T : ModNPC
        {
            AQMod.census.Call("TownNPCCondition", ModContent.NPCType<T>(), canSpawn);
        }
        internal static void SetupContent(AQMod aQMod)
        {
            if (!AQMod.census.IsActive)
            {
                return;
            }
            try
            {
                AddNPC<Robster>("Defeat Crabson!");
                AddNPC<Physicist>("Defeat Omega Starite!");
                AddNPC<Memorialist>("Upgrade an item at the Gore Nest!");
            }
            catch (Exception ex)
            {
                AQMod.Instance.Logger.Warn("There was an error when loading Discord Rich Presence support!", ex);
            }
        }
    }
}