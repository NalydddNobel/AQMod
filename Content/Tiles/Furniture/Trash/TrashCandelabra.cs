using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Tiles.Furniture.Trash;

internal class TrashCandelabra(UnifiedFurniture parent) : InstancedFurnitureCandelabra(parent, default) {
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        if (TrashFurniture.CheckLightFlicker(i, j, NextStyleWidth)) {
            base.ModifyLight(i, j, ref r, ref g, ref b);
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        return TrashFurniture.DrawLightFlickerTile(spriteBatch, Type, AequusTextures.TrashCandelabra_Flame, i, j, NextStyleWidth);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) { }
}
