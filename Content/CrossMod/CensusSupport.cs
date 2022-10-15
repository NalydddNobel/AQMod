using Aequus.NPCs.Friendly.Town;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class CensusSupport : ModSystem
    {
        public static Mod Census { get; private set; }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod("Census");
        }

        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("Census", out var census))
            {
                Census = census;
                census.Call("TownNPCCondition", ModContent.NPCType<Carpenter>(), "Build a house with 4 unique furniture objects (Excluding tables, chairs, and torches)");
                census.Call("TownNPCCondition", ModContent.NPCType<Exporter>(), "Defeat Crabson");
                census.Call("TownNPCCondition", ModContent.NPCType<Occultist>(), "Complete a Demon Siege");
                census.Call("TownNPCCondition", ModContent.NPCType<Physicist>(), "Defeat Omega Starite");
            }
        }

        public override void Unload()
        {
            Census = null;
        }
    }
}