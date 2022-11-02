using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class SnowgraveDebuff : ModBuff
    {
        public override string Texture => Aequus.Debuff;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.StatSpeed() *= 0.75f;
        }
    }
}