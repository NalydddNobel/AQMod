using Aequus.Common.DataSets;
using Aequus.Common.GUI;
using Aequus.Common.UI;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Content.Systems.PotionAffixes;

public class PotionAffixGlobalItem : GlobalItem {
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.IsPotion.Contains(entity.type);
    }

    public override void Load() {
        On_Item.CanApplyPrefix += On_Item_CanApplyPrefix;
        On_Item.CanHavePrefixes += On_Item_CanHavePrefixes;
    }

    private bool On_Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
        return ItemSets.IsPotion.Contains(self.type) || orig(self);
    }

    private static bool On_Item_CanApplyPrefix(On_Item.orig_CanApplyPrefix orig, Item self, int prefix) {
        if (PrefixLoader.GetPrefix(prefix) is UnifiedPotionAffix potionAffix && ItemSets.IsPotion.Contains(self.type)) {
            return potionAffix.CanRoll(self);
        }

        return orig(self, prefix);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (PrefixLoader.GetPrefix(item.prefix) is not UnifiedPotionAffix) {
            return;
        }

        tooltips.RemoveAll(t => t.Mod == "Terraria" && t.Name.StartsWith("Prefix"));
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