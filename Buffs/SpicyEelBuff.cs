using AQMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class SpicyEelBuff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().spicyEel = true;
            player.moveSpeed += 0.1f;
            player.maxRunSpeed *= 1.1f;
            player.meleeSpeed += 0.1f;
            player.pickSpeed += 0.1f;
            player.tileSpeed += 0.1f;
            player.wallSpeed += 0.1f;
        }
    }
}