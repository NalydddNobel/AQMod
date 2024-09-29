using Aequus.Common.DataSets;
using Aequus.Common.GUI;
using Aequus.Common.UI;
using Terraria.GameContent;

namespace Aequus.Content.Entities.PotionAffixes;

public class PotionAffixGlobalItem : GlobalItem {
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.IsPotion.Contains(entity.type);
    }

    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (InventoryUI.ContextsInv.Contains(AequusUI.CurrentItemSlot.Context) && Main.mouseItem?.ModItem is PotionAffixItem affixItem && affixItem.CanApplyPotionAffix(item)) {
            Texture2D texture = TextureAssets.InventoryBack16.Value;
            spriteBatch.Draw(texture, position, null, drawColor.MultiplyRGBA(new Color(50, 50, 50, 20)), 0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }

        return true;
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is UnifiedPotionAffix potionPrefix) {
            Texture2D texture = potionPrefix.GlintTexture!.Value;
            spriteBatch.Draw(texture, position, null, drawColor.MultiplyRGBA(new Color(255, 255, 255, 180)), 0f, texture.Size() / 2f, Main.inventoryScale * 1.2f, SpriteEffects.None, 0f);
        }
    }
}