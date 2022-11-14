using Aequus.Items.Consumables.SoulGems;
using Aequus.Items.Prefixes;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace Aequus.Items.Weapons
{
    public abstract class SoulGemWeaponBase : ModItem
    {
        public const int MaxTier = 4;
        public const int MinTier = 1;

        public int OriginalTier;
        public int tier;
        protected Vector2 ammoDrawOffset = new Vector2(8f, -24f);

        public void ClearSoulFields()
        {
            tier = OriginalTier;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(tooltips.GetIndex("Tooltip#"),
                new TooltipLine(Aequus.Instance, "TooltipSoulGem", AequusText.GetTextWith("ItemTooltip.Common.SoulGemCost", new { SoulTier = GetTierName(tier), })));
            if (tier != OriginalTier)
            {
                int value = tier - OriginalTier;
                tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"),
                    new TooltipLine(Aequus.Instance, "PrefixSoulGemTier", AequusText.GetText("Prefixes.SoulGemTier", $"{(value > 0f ? " + " : "")}{value}"))
                    { IsModifier = true, IsModifierBad = value < 0, });
            }
            //TooltipsGlobalItem.PercentageModifier(soulLimit, OriginalSoulLimit, "PrefixSoulLimit", tooltips, higherIsGood: true);
        }

        public override bool CanUseItem(Player player)
        {
            return FindUsableSoulGem(player) != null;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return SoulWeaponPrefix.ChoosePrefix(Item, rand).Type;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.playerInventory)
                return;

            var center = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
            center.X -= TextureAssets.InventoryBack.Value.Width / 2f * Main.inventoryScale;
            center.Y += TextureAssets.InventoryBack.Value.Height / 2f * Main.inventoryScale;
            int count = 0;
            var player = Main.LocalPlayer;
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                if (!player.inventory[i].IsAir && player.inventory[i].ModItem is SoulGemBase soulGem && soulGem.filled >= tier)
                {
                    count += player.inventory[i].stack;
                }
            }
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (!player.bank4.item[i].IsAir && player.bank4.item[i].ModItem is SoulGemBase soulGem && soulGem.filled >= tier)
                {
                    count += player.bank4.item[i].stack;
                }
            }

            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, count.ToString(), center + ammoDrawOffset * Main.inventoryScale, Color.White, 0f, Vector2.Zero, new Vector2(1f) * Main.inventoryScale * 0.8f, spread: Main.inventoryScale);
        }

        public Item FindUsableSoulGem(Player player)
        {
            Item bestChoice = null;
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                if (!player.inventory[i].IsAir && player.inventory[i].ModItem is SoulGemBase soulGem && soulGem.filled >= tier)
                {
                    if (soulGem.Tier != tier)
                    {
                        bestChoice = player.inventory[i];
                        continue;
                    }
                    return player.inventory[i];
                }
            }
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (!player.bank4.item[i].IsAir && player.bank4.item[i].ModItem is SoulGemBase soulGem && soulGem.filled >= tier)
                {
                    if (soulGem.Tier != tier)
                    {
                        bestChoice = player.bank4.item[i];
                        continue;
                    }
                    return player.bank4.item[i];
                }
            }
            return bestChoice;
        }

        public static string GetTierName(int tier)
        {
            return tier >= MinTier && tier <= MaxTier ? AequusText.GetText($"SoulGemTier.{tier}") : AequusText.Unknown;
        }
    }
}