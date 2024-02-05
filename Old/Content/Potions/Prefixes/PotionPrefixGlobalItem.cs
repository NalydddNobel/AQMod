using Aequus.Content.DataSets;
using Aequus.Old.Content.Potions.Prefixes.BoundedPotions;
using System;

namespace Aequus.Old.Content.Potions.Prefixes;
public class PotionPrefixGlobalItem : GlobalItem {
    internal float _reforgeAnimation;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.Potions.Contains(entity.type);
    }

    public override bool? UseItem(Item item, Player player) {
        if (item.buffType > 0 && item.buffTime > 0 && player.TryGetModPlayer(out PotionsPlayer boundedPotions)) {
            if (PrefixLoader.GetPrefix(item.prefix) is BoundedPrefix && !Main.persistentBuff[item.buffType]) {
                boundedPotions.BoundedPotionIds.Add(item.buffType);

                // Cap buff time so you can't abuse Stuffed potions.
                int buffIndex = player.FindBuffIndex(item.buffType);
                if (buffIndex != -1) {
                    player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex], item.buffTime);
                }
            }
            else {
                boundedPotions.BoundedPotionIds.Remove(item.buffType);
            }
        }

        return null;
    }

    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        return true;
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is PotionPrefix potionPrefix) {
            Texture2D texture = potionPrefix.GlintTexture.Value;
            spriteBatch.Draw(texture, position, null, Utils.MultiplyRGBA(drawColor, new Color(255, 255, 255, 180)), 0f, texture.Size() / 2f, Main.inventoryScale * 1.2f, SpriteEffects.None, 0f);
        }

        if (_reforgeAnimation <= 0f) {
            return;
        }

        float time = _reforgeAnimation - Main.GlobalTimeWrappedHourly + 60f;
        if (time <= 0f || time > 61f || !Main.playerInventory) {
            _reforgeAnimation = 0f;
            return;
        }

        time /= 60f;
    }
}
