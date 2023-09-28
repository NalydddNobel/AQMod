using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes.Scorching {
    public class AncientScorchingDye : DyeItemBase {
        public override string Pass => "RedSpritePass";
        public override int Rarity => ItemRarityID.Orange;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ScorchingDye>();
        }

        public override ArmorShaderData CreateShaderData() {
            return new ArmorShaderData(Effect, Pass).UseColor(new Color(140, 0, 21, 255));
        }
    }
}