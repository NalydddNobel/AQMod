using Aequus.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class AequusBuff : GlobalBuff
    {
        public static HashSet<int> IsWellFedButDoesntIncreaseLifeRegen { get; private set; }

        public override void Load()
        {
            IsWellFedButDoesntIncreaseLifeRegen = new HashSet<int>();
            On.Terraria.NPC.AddBuff += NPC_AddBuff;
        }

        private static void NPC_AddBuff(On.Terraria.NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
        {
            if (Main.debuff[type])
            {
                Player player = AequusPlayer.FindPlayerContext();

                if (player != null)
                {
                    time = (int)(time * player.Aequus().enemyDebuffDuration);
                }

            }
            orig(self, type, time, quiet);
        }

        public static bool AddStaticImmunity(int npc, params int[] buffList)
        {
            return AddStaticImmunity(npc, false, buffList);
        }
        public static bool AddStaticImmunity(int npc, bool isWhipBuff, params int[] buffList)
        {
            if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(npc, out var value))
            {
                NPCID.Sets.DebuffImmunitySets.Add(npc, new NPCDebuffImmunityData() { SpecificallyImmuneTo = buffList });
                return true;
            }

            if (value == null)
            {
                value = NPCID.Sets.DebuffImmunitySets[npc] = new NPCDebuffImmunityData();
            }

            if (value.ImmuneToAllBuffsThatAreNotWhips && !isWhipBuff)
            {
                return true;
            }

            if (value.SpecificallyImmuneTo == null)
            {
                value.SpecificallyImmuneTo = buffList;
                return true;
            }

            var list = new List<int>(buffList);
            for (int i = 0; i < value.SpecificallyImmuneTo.Length; i++)
            {
                list.Remove(value.SpecificallyImmuneTo[i]);
            }
            buffList = list.ToArray();
            if (buffList.Length <= 0)
            {
                return true;
            }

            Array.Resize(ref value.SpecificallyImmuneTo, value.SpecificallyImmuneTo.Length + buffList.Length);
            int k = 0;
            for (int j = value.SpecificallyImmuneTo.Length - buffList.Length; j < value.SpecificallyImmuneTo.Length; j++)
            {
                value.SpecificallyImmuneTo[j] = buffList[k];
                k++;
            }
            return true;
        }
    }
}