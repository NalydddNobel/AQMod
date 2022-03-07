using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public sealed class NPCTemperature : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public sbyte temperature;

        public override void AI(NPC npc)
        {
        }

        public void ChangeTemperature(NPC npc, sbyte newTemperature)
        {
            if (npc.GetGlobalNPC<AQNPC>().hotDamage && newTemperature > 0)
            {
                newTemperature /= 2;
            }
            else if (npc.coldDamage && newTemperature < 0)
            {
                newTemperature /= 2;
            }
            if (temperature < 0)
            {
                if (newTemperature < 0)
                {
                    if (temperature > newTemperature)
                        temperature = newTemperature;
                }
                else
                {
                    temperature = 0;
                }
            }
            else if (temperature > 0)
            {
                if (newTemperature > 0)
                {
                    if (temperature < newTemperature)
                        temperature = newTemperature;
                }
                else
                {
                    temperature = 0;
                }
            }
            else
            {
                temperature = newTemperature;
            }
            if (newTemperature < 0)
            {
                temperature--;
            }
            else
            {
                temperature++;
            }
        }
    }
}