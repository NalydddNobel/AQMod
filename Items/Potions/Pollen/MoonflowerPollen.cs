using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Content.Tiles.HangingPots;
using Aequus.CrossMod;
using Aequus.Particles.Dusts;

namespace Aequus.Items.Potions.Pollen;
public class MoonflowerPollen : ModItem {
    public override void Load() {
        Mod.AddContent(new InstancedHangingPot(this, "MoonflowerPot", AequusTextures.MoonflowerPot.FullPath));
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.PixieDust);
        Item.rare = ItemRarityID.Green;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 200);
    }

    public override void PostUpdate() {
        Lighting.AddLight(Item.Center, Color.BlueViolet.ToVector3() * 0.5f);
        if (Item.timeSinceItemSpawned % 30 == 0 || Main.rand.NextBool(12)) {
            var d = Dust.NewDustDirect(Item.position, Item.width, Item.height, ModContent.DustType<MonoDust>(), newColor: Color.BlueViolet.UseA(128) * Main.rand.NextFloat(0.33f, 2f));
            d.velocity.X *= 0.6f;
            d.velocity.Y *= 0.35f;
            d.velocity.Y += Main.rand.NextFloat(-4.5f, -2f);
            d.fadeIn = d.scale;
            d.scale *= 0.5f;
        }
    }

    public override void AddRecipes() {
        var prefix = PrefixLoader.GetPrefix(ModContent.PrefixType<DoubledTimePrefix>());
        for (int i = 0; i < ItemLoader.ItemCount; i++) {
            if (prefix.CanRoll(ContentSamples.ItemsByType[i])) {
                var r = Recipe.Create(i, 1)
                    .ResultPrefix<DoubledTimePrefix>()
                    .AddIngredient(i)
                    .AddIngredient(Type)
                    .DisableDecraft()
                    .TryRegisterAfter(i);
                MagicStorage.AddBlacklistedItemData(i, prefix.Type);
            }
        }
    }
}