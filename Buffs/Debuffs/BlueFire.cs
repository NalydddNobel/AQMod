using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class BlueFire : ModBuff
    {
        public static SoundStyle InflictDebuffSound = new SoundStyle(Aequus.AssetsPath + "Sounds/inflict_bluefire");

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}