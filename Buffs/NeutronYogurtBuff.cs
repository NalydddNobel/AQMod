using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class NeutronYogurtBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().neutronYogurt = true;
            player.wellFed = true;
            player.statDefense += 3;
            player.allDamage += 0.075f;
            player.meleeSpeed += 0.075f;

            player.meleeCrit += 3;
            player.rangedCrit += 3;
            player.magicCrit += 3;
            player.thrownCrit += 3;

            player.minionKB += 0.75f;
            player.moveSpeed += 0.3f;

            player.gravity *= 0.9f;
        }
    }
}