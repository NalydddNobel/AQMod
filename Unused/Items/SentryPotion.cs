using Aequus.Common;
using Aequus.Items;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class SentryPotion : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(208, 101, 32, 0), new Color(241, 216, 109, 0), new Color(138, 76, 31, 0), };
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.SummoningPotion);
            Item.rare = ItemRarityID.Blue;
        }
    }
}