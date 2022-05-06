using Aequus.Common.Catalogues;
using Aequus.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class NecromancyDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            NecromancyTypes.NecromancyDebuffs.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<PlayerZombie>().zombieDrain = 5 * AequusHelpers.NPCREGEN;
        }

        public static void ApplyDebuff(NPC npc, int time, int player, float tier)
        {
            if (tier >= 100 || (NecromancyTypes.NPCs.TryGetValue(npc.type, out var value) && value.PowerNeeded <= tier))
            {
                npc.AddBuff(ModContent.BuffType<NecromancyDebuff>(), time);
                npc.GetGlobalNPC<PlayerZombie>().zombieOwner = player;
                npc.GetGlobalNPC<PlayerZombie>().zombieDebuffTier = tier;
            }
        }
    }
}