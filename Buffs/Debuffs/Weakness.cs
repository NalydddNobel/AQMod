using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class Weakness : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Weak;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}