using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Foods
{
    public class GrapePhanta : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}