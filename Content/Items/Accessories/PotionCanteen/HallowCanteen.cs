﻿using Aequus.Common.Graphics.Shaders;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.PotionCanteen;

public class HallowCanteen : UnifiedCanteen {
    public HallowCanteen() : base(new CanteenInfo(
        MaxBuffs: 2,
        PotionsRequiredToAddBuff: 15,
        Rarity: ItemRarityID.Orange,
        Value: Item.buyPrice(gold: 10)
    )) { }

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(_info.MaxBuffs, _info.PotionsRequiredToAddBuff);

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        spriteBatch.Draw(AequusTextures.HallowCanteenEmpty, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        if (HasBuffs()) {
            spriteBatch.End();
            spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);

            float a = drawColor.A > 0 ? drawColor.A / 255f : Main.inventoryBack.A / 255f;
            DrawLiquid(spriteBatch, position, frame, Color.White, 0f, origin, scale);

            spriteBatch.End();
            spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
        }

        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
        var position = Item.Center - Main.screenPosition - new Vector2(0f, 4f);
        var origin = frame.Size() / 2f;
        spriteBatch.Draw(AequusTextures.HallowCanteenEmpty, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);

        if (HasBuffs() && !Item.shimmerWet) {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: true);

            DrawLiquid(spriteBatch, position, frame, lightColor, rotation, origin, scale);

            spriteBatch.End();
            spriteBatch.Begin_World();
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