using System;
using Terraria;
using Terraria.ID;

namespace Aequus.Common
{
    [Obsolete]
    public struct BestiarySpawnInfo
    {
        public static int ValueConversionRate => Item.gold * 2;

        private readonly int npc;
        public int CrystalValue;
        public int Value;
        public bool Enabled;

        public BestiarySpawnInfo(int npcNetID, bool extractNPC = true)
        {
            npc = npcNetID;
            CrystalValue = 1;
            Value = 0;
            Enabled = true;
            if (extractNPC)
            {
                ExtractNPC(ContentSamples.NpcsByNetId[npcNetID]);
            }
        }
        public BestiarySpawnInfo(int npcNetID, BestiarySpawnInfo clone) : this(npcNetID, extractNPC: false)
        {
            npc = npcNetID;
            Value = clone.Value;
        }

        public void ExtractNPC(NPC npcInstance)
        {
            Value = (int)npcInstance.value;
            CrystalValue = (int)Math.Max(Value / (float)ValueConversionRate, 1);
        }
    }
}