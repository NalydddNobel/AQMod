using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class PickBreak : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.pickSpeed *= 2;
            player.Aequus().pickTileDamage *= 0.5f;
        }
    }

    public class PickBreakPlayer : ModPlayer
    {
        public bool pickBreak;

        public override void Load()
        {
            On.Terraria.Player.PickTile += Player_PickTile;
        }

        private static void Player_PickTile(On.Terraria.Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
        {
            if (self.GetModPlayer<PickBreakPlayer>().pickBreak)
            {
                pickPower /= 2;
            }
            orig(self, x, y, pickPower);
        }

        public override void ResetEffects()
        {
            pickBreak = false;
        }
    }
}