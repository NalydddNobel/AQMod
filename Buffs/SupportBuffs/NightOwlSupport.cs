using Terraria;
using Terraria.ID;

namespace AQMod.Buffs.SupportBuffs
{
    public class NightOwlSupport : BuffSupportType
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.BuffSupportCheck();
            aQPlayer.BuffSupport[BuffID.NightOwl] = 1;
        }
    }
}