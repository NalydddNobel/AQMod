using Aequus.NPCs.Friendly.Town;
using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class Census : ModSupport<Census>
    {
        private void AddTownNPC<T>(string text) where T : ModNPC
        {
            try
            {
                Instance.Call("TownNPCCondition", ModContent.NPCType<T>(), text);
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void PostSetupContent()
        {
            if (Instance == null)
                return;

            AddTownNPC<Carpenter>("Build a house with 4 unique furniture objects (Excluding tables, chairs, and torches)");
            AddTownNPC<Exporter>("Defeat Crabson");
            AddTownNPC<Occultist>("Complete a Demon Siege");
            AddTownNPC<Physicist>("Defeat an Ultra Starite");
        }
    }
}