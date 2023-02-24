using Aequus.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class BlueFire : ModBuff
    {
        public static SoundStyle InflictDebuffSound => Aequus.GetSound("inflictFire", variance: 0.3f);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.PlayerDoTBuff.Add(Type);
        }
    }
}