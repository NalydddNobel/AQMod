using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials {
    public class ShimmerMaterial : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 5);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White with { A = 200 };
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            if (Item.timeSinceItemSpawned > 180) {
                Item.active = false;
                Item.TurnToAir();
            }
            if (Item.timeSinceItemSpawned % 2 == 0) {
                var d = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.PinkFairy, Alpha: 100);
                d.velocity = Vector2.Normalize(d.position - Item.Center) * (Main.rand.NextFloat(2f) + 1f);
                d.noGravity = true;
                d.fadeIn = d.scale + 0.5f;
            }
        }
    }
}