using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class TonicSpawnratesBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, Color.Green);
        }
    }
}