using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Summon
{
    public class RustyKnife : ModItem, IUpdateVanity
    {
        public override void SetDefaults()
        {
            item.damage = 121;
            item.summon = true;
            item.width = 20;
            item.height = 20;
            item.rare = AQItem.RarityDedicatedItem;
            item.accessory = true;
            item.value = Item.sellPrice(gold: 6);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria")
                {
                    if (tooltips[i].Name == "Knockback")
                    {
                        tooltips.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.omori = true;
            if (!hideVisual)
                aQPlayer.omoriEffect = true;
        }

        void IUpdateVanity.UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            if (i > AQPlayer.MaxDye)
                aQPlayer.omoriEffect = true;
        }
    }
}