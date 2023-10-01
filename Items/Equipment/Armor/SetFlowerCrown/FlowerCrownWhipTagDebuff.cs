using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetFlowerCrown {
    public class FlowerCrownWhipTagDebuff : ModBuff {
        public override string Texture => AequusTextures.Debuff.Path;

        public override void SetStaticDefaults() {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}