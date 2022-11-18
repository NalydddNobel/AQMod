using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class NecromancyPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(75, 175, 29));
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Aequus().ghostSlotsMax++;
        }
    }
}