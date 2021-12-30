using AQMod.Content.Players;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Vampire
{
    public class VampirismDay : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<VampirismPlayer>().daylightBurning = true;
        }
    }
}