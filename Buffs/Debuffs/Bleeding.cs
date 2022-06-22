using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class Bleeding : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Bleeding;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}