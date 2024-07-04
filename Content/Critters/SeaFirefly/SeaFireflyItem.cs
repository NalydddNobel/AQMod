using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Graphics.Textures;
using Terraria.GameContent;
using Terraria.Localization;

namespace AequusRemake.Content.Critters.SeaFirefly;

internal class SeaFireflyItem(UnifiedCritter Parent, string DyeName, byte Color) : InstancedCritterItem(Parent, DyeName) {
    public readonly byte Color = Color;
    public readonly string DyeName = DyeName;

    public override LocalizedText DisplayName => Critter.GetLocalization($"DisplayName.{DyeName}", () => $"{DyeName} Sea Firefly");

    public ISeaFireflyInstanceData Current => SeaFireflyRegistry.GetPalette(Color);

    public override void SetStaticDefaults() {
        if (!Main.dedServ && Current.ColorEffect != null) {
            Main.QueueMainThreadAction(SetTexture);
        }
    }

    void SetTexture() {
        IColorEffect effect = SeaFireflyRegistry.SeaFireflyItemMaskEffect.WithEffect(Current.ColorEffect);
        TextureAssets.Item[Type] = TextureGen.PerPixel(effect, TextureAssets.Item[Type]);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        return true;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        return true;
    }
}
