using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class Harmony : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.magicCrit += 5;
            player.magicDamage += 0.15f;
            player.statManaMax2 += 40;
            int clairvoyanceBuff = player.FindBuffIndex(BuffID.Clairvoyance);
            if (clairvoyanceBuff != -1)
            {
                player.DelBuff(clairvoyanceBuff);
                buffIndex--;
            }
        }
    }
}