using Terraria.GameContent;

namespace Aequus.Old.Content.Equipment.Accessories.PotionCanteen;

public class PotionCanteen : TemplateCanteen {
    public override int Rarity => ItemRarityID.LightRed;
    public override int Value => Item.buyPrice(gold: 10);

    public override int PotionsContained => 2;
    public override int PotionRecipeRequirement => 15;

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        if (!HasBuffs) {
            return true;
        }
        var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
        var liquidFrame = GetLiquidFrame(liquidTexture);
        var liquidColor = GetPotionColors();
        float a = drawColor.A > 0 ? drawColor.A / 255f : Main.inventoryBack.A / 255f;
        spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor * a, 0f, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, liquidColor with { A = 255 }, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
        var position = Item.Center - Main.screenPosition;
        var origin = frame.Size() / 2f;
        spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
        if (HasBuffs) {
            var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
            var liquidFrame = GetLiquidFrame(liquidTexture);
            var liquidColor = GetPotionColors();
            spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, frame, ExtendLight.Get(Item.Center).MultiplyRGBA(liquidColor), rotation, origin, scale, SpriteEffects.None, 0f);
        }
        return false;
    }

    private static Rectangle GetLiquidFrame(Texture2D liquidTexture) {
        return liquidTexture.Frame(verticalFrames: 16, frameY: (int)Main.GameUpdateCount / 7 % 15);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ItemID.CrystalShard, 20)
            .AddIngredient(ItemID.PixieDust, 30)
            .Register();

        base.AddRecipes();
    }
}