using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Aequus.Buffs.Debuffs
{
    public class SoulStolen : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        //public override void Update(NPC npc, ref int buffIndex)
        //{
        //    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Gold);
        //}
    }
}