using Aequus.Content.DataSets;
using Aequus.Old.Content.Potions.Prefixes.BoundedPotions;
using Aequus.Old.Content.Potions.Prefixes.EmpoweredPotions;
using Aequus.Old.Content.Potions.Prefixes.SplashPotions;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Aequus.Old.Content.Potions.Prefixes;
public class PotionPrefixGlobalItem : GlobalItem {
    internal float _reforgeAnimation;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.Potions.Contains(entity.type);
    }

    public override bool CanUseItem(Item item, Player player) {
        if (PotionsPlayer.UsingQuickBuffHack && item.prefix >= PrefixID.Count && item.buffType > 0 && item.buffTime > 0) {
            ModPrefix prefix = PrefixLoader.GetPrefix(item.prefix);
            // Prevent the player from quick-buffing any other empowered potions
            // if they already have an empowered potion active
            if (prefix is EmpoweredPrefix && player.TryGetModPlayer(out PotionsPlayer potionPlayer) && potionPlayer.empoweredPotionId != 0) {
                return false;
            }

            // Prevent using the potion if it's a splash potion
            if (prefix is SplashPrefix) {
                return false;
            }
        }
        return base.CanUseItem(item, player);
    }

    public override bool? UseItem(Item item, Player player) {
        if (item.buffType > 0 && item.buffTime > 0 && player.TryGetModPlayer(out PotionsPlayer potionPlayer)) {
            BoundedPrefix.CheckUseItem(item, player, potionPlayer);
            EmpoweredPrefix.CheckUseItem(item, player, potionPlayer);
        }

        return null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is EmpoweredPrefix) {
            if (EmpoweredBuffs.Override.TryGetValue(item.buffType, out var buffOverride) && buffOverride.Tooltip != null) {
                tooltips.AddTooltip(new TooltipLine(Mod, "TooltipEmpowered", buffOverride.Tooltip.Value));
            }
        }
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
