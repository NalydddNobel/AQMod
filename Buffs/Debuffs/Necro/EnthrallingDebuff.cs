using Aequus.Content.Necromancy;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class EnthrallingDebuff : NecromancyDebuff
    {
        public override string Texture => AequusHelpers.GetPath<NecromancyDebuff>();

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NecromancyNPC>().zombieDrain = 100 * AequusHelpers.NPCREGEN;
        }

        public static void ApplyDebuffTest(NPC npc, int time, int player, float tier)
        {
            npc.buffImmune[ModContent.BuffType<EnthrallingDebuff>()] = false;
            npc.AddBuff(ModContent.BuffType<EnthrallingDebuff>(), time);
            npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = player;
            npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = tier;
        }
    }
}