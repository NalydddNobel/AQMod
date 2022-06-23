using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly
{
    public class TownNPCBuffs : GlobalNPC
    {
        public override void BuffTownNPC(ref float damageMult, ref int defense)
        {
            if (AequusSystem.downedCrabson)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
            if (AequusSystem.downedOmegaStarite)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
        }
        public void GenericBuffs(ref float damageMult, ref int defense)
        {
            damageMult += 0.1f;
            defense += 3;
        }
    }
}