using Aequus.Common.Networking;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class DamageOverTime : GlobalNPC, IEntityNetworker
    {
        public bool hasLocust;
        public int locustStacks;

        public override bool InstancePerEntity => true;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!hasLocust)
            {
                locustStacks = 0;
            }
            else
            {
                int dot = Math.Min(locustStacks, 20);

                npc.AddRegen(-dot);
                if (damage < dot)
                    damage = dot;
            }
        }

        void IEntityNetworker.Send(int whoAmI, BinaryWriter writer)
        {
            writer.Write(hasLocust);
            if (hasLocust)
            {
                writer.Write((byte)locustStacks);
            }
        }

        void IEntityNetworker.Receive(int whoAmI, BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                hasLocust = true;
                locustStacks = reader.ReadByte();
            }
        }
    }
}