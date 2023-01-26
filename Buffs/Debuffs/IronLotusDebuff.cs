using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class IronLotusDebuff : ModBuff
    {
        public override string Texture => Aequus.Debuff;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.IsFire.Add(Type);
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}