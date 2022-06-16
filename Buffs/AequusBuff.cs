using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class AequusBuff : ILoadable
    {
        public static HashSet<int> CannotClear { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            CannotClear = new HashSet<int>();
            On.Terraria.Main.TryRemovingBuff += Main_TryRemovingBuff;
        }

        private void Main_TryRemovingBuff(On.Terraria.Main.orig_TryRemovingBuff orig, int i, int b)
        {
            if (CannotClear.Contains(b))
            {
                return;
            }

            orig(i, b);
        }

        void ILoadable.Unload()
        {
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