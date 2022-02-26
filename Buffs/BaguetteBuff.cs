using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class BaguetteBuff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.allDamage += 0.01f;
        }
    }
}