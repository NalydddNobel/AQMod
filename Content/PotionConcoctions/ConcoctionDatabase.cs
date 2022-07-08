using Aequus.Buffs;
using Aequus.Items.Consumables.BuffPotions;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.PotionConcoctions
{
    public class ConcoctionDatabase : ModSystem
    {
        public static HashSet<int> ConcoctibleBuffsBlacklist { get; private set; }

        public override void PostSetupContent()
        {
            var l = new List<int>();
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (BuffID.Sets.IsWellFed[i])
                {
                    l.Add(i);
                }
            }
            ConcoctibleBuffsBlacklist = new HashSet<int>(l)
            {
                BuffID.Tipsy,
                BuffID.Honey,
                BuffID.Lucky,
                ModContent.BuffType<Vampirism>(),
            };
        }

        public static bool ConcoctiblePotion(Item item)
        {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.meleeBuff[item.buffType] &&
                !ConcoctibleBuffsBlacklist.Contains(item.buffType) && !(item.ModItem is ConcoctionResult);
        }
    }
}