using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class SentryBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(175, 75, 29));
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.maxTurrets++;
        }
    }
}