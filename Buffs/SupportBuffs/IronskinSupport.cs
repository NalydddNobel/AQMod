using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Buffs.SupportBuffs
{
    public class IronskinSupport : BuffSupportType
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.BuffSupportCheck();
            aQPlayer.BuffSupport[BuffID.Ironskin] = 1;
        }
    }
}