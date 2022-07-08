using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class BloodthirstBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffColorDatabase.BuffToColor.Add(Type, new Color(200, 60, 255, 255));
        }
    }
}