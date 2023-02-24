using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Town
{
    public class TownNPCBuffs : GlobalNPC
    {
        public void GenericBuffs(ref float damageMult, ref int defense)
        {
            damageMult += 0.1f;
            defense += 3;
        }

        public override void BuffTownNPC(ref float damageMult, ref int defense)
        {
            if (AequusWorld.downedCrabson)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
            if (AequusWorld.downedOmegaStarite)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
            if (AequusWorld.downedDustDevil)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
        }
    }
}