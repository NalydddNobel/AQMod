

using Terraria.GameContent;

namespace Aequus.Common.Entities.Items;

public interface IHaveCrossedOutIndicator {
    bool Active();
}

public class CrossedOutIndicator : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is IHaveCrossedOutIndicator;
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (item.ModItem is not IHaveCrossedOutIndicator crossedOut || !crossedOut.Active()) {
            return;
        }

        spriteBatch.DrawAlign(TextureAssets.Cd.Value, position, null, drawColor, 0f, Main.inventoryScale, SpriteEffects.None);
    }
}
