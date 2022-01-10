using AQMod.NPCs.Friendly;
using System;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    public static class CensusSupport
    {
        public static Mod GetMod() => ModLoader.GetMod("Census");

        public static void AddSupport(AQMod aQMod)
        {
            try
            {
                var census = GetMod();
                if (census != null)
                {
                    census.Call("TownNPCCondition", ModContent.NPCType<Robster>(), "Defeat Jerry Crabson!");
                    census.Call("TownNPCCondition", ModContent.NPCType<Physicist>(), "Defeat Omega Starite!");
                    census.Call("TownNPCCondition", ModContent.NPCType<Memorialist>(), "Upgrade an item at the Gore Nest!");
                    census.Call("TownNPCCondition", ModContent.NPCType<BalloonMerchant>(), "Can be found scouring the skies during windy days!");
                }
            }
            catch (Exception ex)
            {
                AQMod.GetInstance().Logger.Warn("There was an error when loading Discord Rich Presence support!", ex);
            }
        }
    }
}
