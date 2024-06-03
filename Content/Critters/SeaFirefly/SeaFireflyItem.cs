using Aequus.Core.ContentGeneration;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Content.Critters.SeaFirefly;

internal class SeaFireflyItem(UnifiedCritter Parent, string DyeName, byte Color) : InstancedCritterItem(Parent, DyeName) {
    public readonly byte Color = Color;
    public readonly string DyeName = DyeName;

    public override LocalizedText DisplayName => Critter.GetLocalization($"DisplayName.{DyeName}", () => $"{DyeName} Sea Firefly");

    public ISeaFireflyInstanceData Current => SeaFirefly.GetPalette(Color);

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.SeaFireflyItem_Dyed, position, frame, Current.GetBugColor() with { A = 255 }, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
        Vector2 position = ExtendItem.WorldDrawPos(Item, frame);
        Vector2 origin = frame.Size() / 2f;

        spriteBatch.Draw(texture, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.SeaFireflyItem_Dyed, position, frame, Utils.MultiplyRGBA(lightColor, Current.GetBugColor() with { A = 255 }), rotation, origin, scale, SpriteEffects.None, 0f);
        return false;
    }
}
