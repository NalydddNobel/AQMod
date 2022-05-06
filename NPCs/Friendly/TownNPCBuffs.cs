using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly
{
    public class TownNPCBuffs : GlobalNPC
    {
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
        }
        public void GenericBuffs(ref float damageMult, ref int defense)
        {
            damageMult += 0.1f;
            defense += 3;
        }
    }
}