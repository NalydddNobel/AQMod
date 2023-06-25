using Aequus.Common;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class NecromancyPotion : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.HotPink.UseA(50), };
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.SummoningPotion);
            Item.rare = ItemRarityID.Blue;
        }
    }
}