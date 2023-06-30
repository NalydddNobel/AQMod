using Aequus.Common.Buffs;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs {
    public class ManathirstBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(110, 61, 255));
            AequusBuff.AddPotionConflict(Type, BuffID.ManaRegeneration);
            AequusBuff.AddPotionConflict(Type, ModContent.BuffType<BloodthirstBuff>());
        }
    }
}