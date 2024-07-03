using Aequu2.Core;
using Terraria.DataStructures;

namespace Aequu2.Old.Content.Items.Materials;

public class StariteMaterial : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(7, 13));
        ItemSets.AnimatesAsSoul[Type] = true;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = Commons.Rare.EventGlimmer - 1;
        Item.value = Item.sellPrice(silver: 2);
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