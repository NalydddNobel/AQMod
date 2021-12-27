using Terraria;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool blueFire;
        public bool shimmering;

        internal static void GiveImmunitiesToAllBuffs(NPC npc)
        {
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
        }
    }
}