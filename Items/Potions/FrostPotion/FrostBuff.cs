using Aequus.Common.Buffs;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.FrostPotion; 

public class FrostBuff : ModBuff {
    public override void SetStaticDefaults() {
        LegacyPotionColorsDatabase.BuffToColor.Add(Type, new Color(61, 194, 255));
        AequusBuff.AddPotionConflict(Type, BuffID.Warmth);
    }

    public override void Update(Player player, ref int buffIndex) {
        player.Aequus().potionFrost = true;
    }
}