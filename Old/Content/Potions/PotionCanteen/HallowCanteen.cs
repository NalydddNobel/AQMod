using Aequus.Core.Assets;
using Terraria.GameContent;

namespace Aequus.Old.Content.Potions.PotionCanteen;

public class HallowCanteen : TemplateCanteen {
    public override int Rarity => ItemRarityID.LightRed;
    public override int Value => Item.buyPrice(gold: 10);

    public override int PotionsContained => 2;
    public override int PotionRecipeRequirement => 15;

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(AequusTextures.HallowCanteenEmpty, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        if (HasBuffs) {
            spriteBatch.End();
            spriteBatch.BeginUI(immediate: true, useScissorRectangle: true);

            float a = drawColor.A > 0 ? drawColor.A / 255f : Main.inventoryBack.A / 255f;
            DrawLiquid(spriteBatch, position, frame, Color.White, 0f, origin, scale);

            spriteBatch.End();
            spriteBatch.BeginUI(immediate: false, useScissorRectangle: true);
        }

        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
        var position = Item.Center - Main.screenPosition - new Vector2(0f, 4f);
        var origin = frame.Size() / 2f;
        spriteBatch.Draw(AequusTextures.HallowCanteenEmpty, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);

        if (HasBuffs) {
            spriteBatch.End();
            spriteBatch.BeginWorld(shader: true);

            DrawLiquid(spriteBatch, position, frame, lightColor, rotation, origin, scale);

            spriteBatch.End();
            spriteBatch.BeginWorld();
        }

        return false;
    }

    private void DrawLiquid(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, float scale) {
        var liquidColor = GetPotionColors();

        AequusShaders.LuminentMultiply.Value.CurrentTechnique.Passes[0].Apply();

        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, color.MultiplyRGBA(liquidColor), rotation, origin, scale, SpriteEffects.None, 0f);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<PotionCanteen>())
            .AddIngredient(ItemID.CrystalShard, 50)
            .AddIngredient(ItemID.PixieDust, 30)
            .Register();

        base.AddRecipes();
    }
}