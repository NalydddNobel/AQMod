using Aequu2.Core.ContentGeneration;

namespace Aequu2.Content.Fishing.FishingPoles;

public class BambooFishingPole : UnifiedFishingPole {
    public override bool BobberPreAI(Projectile bobber) {
        if (bobber.ai[1] == 0f && bobber.localAI[1] > 0f && bobber.localAI[1] < 660f && Main.myPlayer == bobber.owner) {
            bobber.localAI[1] += Main.rand.Next(1, 16);
        }
        return true;
    }

    public override void BobberOnKill(Projectile bobber, int timeLeft) {
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.WoodFishingPole);
        Item.fishingPole = 10;
        Item.shootSpeed = 15f;
        Item.rare = ItemRarityID.White;
        Item.value = Item.sellPrice(copper: 80);
        Item.shoot = _bobber.Type;
    }

    public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
        lineOriginOffset = new Vector2(46f, -32f);
        lineColor = new Color(187, 161, 95, 255);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BambooBlock, 8)
            .AddTile(TileID.WorkBenches)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.BambooWorkbench);
    }
}
