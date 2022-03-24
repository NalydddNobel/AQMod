using AQMod.Buffs;
using AQMod.Buffs.Vampire;
using AQMod.Content.Concoctions.Recipes;
using AQMod.Items.Potions.Concoctions;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.Concoctions
{
    public sealed class ConcoctionsSystem
    {
        public readonly Dictionary<int, ConcoctionRecipeBase> RecipeData;
        public readonly HashSet<int> ConcoctibleBuffsBlacklist;

        public ConcoctionsSystem()
        {
            RecipeData = new Dictionary<int, ConcoctionRecipeBase>()
            {
                [ModContent.ItemType<Molite>()] = new ConcoctionItemRecipe<MoliteTag>(),
            };

            ConcoctibleBuffsBlacklist = new HashSet<int>(AQBuff.Sets.Instance.FoodBuff)
            {
                BuffID.Tipsy,
                BuffID.Honey,
                BuffID.Lifeforce,
                ModContent.BuffType<Spoiled>(),
                ModContent.BuffType<Vampirism>(),
            };
        }

        public bool ConcoctiblePotion(Item item)
        {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.EatingUsing
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.meleeBuff[item.buffType] &&
                !ConcoctibleBuffsBlacklist.Contains(item.buffType) && (item.type < Main.maxItemTypes ? true : !item.modItem.CloneNewInstances && !(item.modItem is ConcoctionItem));
        }
    }
}