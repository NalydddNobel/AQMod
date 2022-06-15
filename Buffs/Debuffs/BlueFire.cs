using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class BlueFire : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AequusPlayer>().debuffBlueFire = true;
        }

        //public override void Update(NPC npc, ref int buffIndex)
        //{
        //    npc.GetGlobalNPC<AequusPlayer>().blueFire = true;
        //}
    }
}