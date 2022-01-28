using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class BlueFire : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().notFrostburn = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AQNPC>().blueFire = true;
        }
    }
}
