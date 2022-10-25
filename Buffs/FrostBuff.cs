using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class FrostBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(61, 194, 255));
        }
    }
}