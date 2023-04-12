using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials {
    public class StariteMaterial : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(7, 13));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.value = Item.sellPrice(silver: 5);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            if (Item.timeSinceItemSpawned % 8 == 0) {
                var d = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.YellowTorch, Alpha: 100);
                d.velocity = Vector2.Normalize(d.position - Item.Center) * (Main.rand.NextFloat(2f) + 1f);
                d.noGravity = true;
                d.fadeIn = d.scale + 0.5f;
            }
        }
    }
}