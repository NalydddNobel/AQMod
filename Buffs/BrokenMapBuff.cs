using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class BrokenMapBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BrokenMapPlayer>().noInfoAccessories = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }

    public class BrokenMapBuffGlobalInfoDisplay : GlobalInfoDisplay
    {
        public override bool? Active(InfoDisplay currentDisplay)
        {
            if (Main.LocalPlayer.GetModPlayer<BrokenMapPlayer>().noInfoAccessories)
                return false;
            return null;
        }
    }

    public class BrokenMapPlayer : ModPlayer
    {
        public bool noInfoAccessories;

        public void ResetEffects_BrokenMap()
        {
            noInfoAccessories = false;
        }
    }
}