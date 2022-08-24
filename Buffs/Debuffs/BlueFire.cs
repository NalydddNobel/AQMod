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

        public static bool InflictAndPlaySound(NPC target, int time, int chance = 1)
        {
            if (target.life <= 0 || (chance > 1 && !Main.rand.NextBool(chance)))
            {
                return false;
            }

            bool hasBuff = target.HasBuff<BlueFire>();

            target.AddBuff(ModContent.BuffType<BlueFire>(), 240);

            if (!hasBuff && target.HasBuff<BlueFire>())
            {
                SoundEngine.PlaySound(InflictDebuffSound);
            }
            return target.HasBuff<BlueFire>();
        }
    }
}