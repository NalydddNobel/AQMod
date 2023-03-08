using Aequus.Common.Audio;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class CrimsonHellfire : ModBuff
    {
        public static Color FireColor => new Color(175, 65, 20, 10);
        public static Color BloomColor => new Color(175, 10, 2, 10);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.DemonSiegeEnemyImmunity.Add(Type);
            AequusBuff.IsFire.Add(Type);
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.Aequus().crimsonHellfireStacks++;
            return false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var aequus = npc.Aequus();
            aequus.crimsonHellfireStacks = Math.Max(aequus.crimsonHellfireStacks, (byte)1);
        }

        public static void AddBuff(NPC npc, int time, bool inflictOnFireInsteadIfItsImmune = true)
        {
            if (npc.buffImmune[ModContent.BuffType<CrimsonHellfire>()])
            {
                if (inflictOnFireInsteadIfItsImmune)
                {
                    npc.AddBuff(BuffID.OnFire, time);
                }
                return;
            }

            AequusBuff.ApplyBuff<CrimsonHellfire>(npc, time, out bool canPlaySound);
            if (canPlaySound)
            {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(npc.Center, pitchOverride: -0.2f);
            }
        }
    }
}