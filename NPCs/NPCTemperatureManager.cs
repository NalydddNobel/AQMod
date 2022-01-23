using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public sealed class NPCTemperatureManager : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool hotDamage;
        public sbyte temperature;

        public override void SetDefaults(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.Hellbat:
                case NPCID.Lavabat:
                case NPCID.LavaSlime:
                case NPCID.BlazingWheel:
                case NPCID.FireImp:
                case NPCID.BurningSphere:
                case NPCID.MeteorHead:
                case NPCID.HellArmoredBones:
                case NPCID.HellArmoredBonesMace:
                case NPCID.HellArmoredBonesSpikeShield:
                case NPCID.HellArmoredBonesSword:
                    {
                        hotDamage = true;
                    }
                    break;
            }
        }

        public override void AI(NPC npc)
        {
            if (hotDamage)
            {
                if (temperature < 100)
                    temperature++;
            }
            else if (npc.coldDamage)
            {
                if (temperature > -100)
                    temperature--;
            }
            else
            {
                if (temperature < -100)
                {
                    temperature = -100;
                }
                else if (temperature > 100)
                {
                    temperature = 100;
                }
            }
        }

        public void ChangeTemperature(NPC npc, sbyte newTemperature)
        {
            if (hotDamage && newTemperature > 0)
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