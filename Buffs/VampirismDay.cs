using Aequus.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class VampirismDay : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerVampirism>().daylightBurning = true;
        }
    }
}