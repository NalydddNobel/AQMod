using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Items.Potions.Special
{
    public abstract class ConcoctionResult : ModItem
    {
        public override bool CloneNewInstances => true;

        public Item original;

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["OriginalItem"] = original,
            };
        }

        public override void Load(TagCompound tag)
        {
            original = tag.Get<Item>("OriginalItem");
            SetPotion();
        }

        public virtual void SetPotion()
        {
            //item.CloneWithModdedDataFrom(original);
            //item.CloneDefaults(original.type);
            item.maxStack = 1;
            item.buffType = original.buffType;
            item.buffTime = original.buffTime;
            item.potion = original.potion;
        }

        private static bool CustomDataPotionCheck(Item item)
        {
            if (item.type < Main.maxItemTypes)
            {
                return true;
            }
            return !item.modItem.CloneNewInstances && !(item.modItem is ConcoctionResult);
        }
        public static bool IsValidPotion(Item item)
        {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.EatingUsing
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.meleeBuff[item.buffType] &&
                !AQBuff.Sets.Instance.NoStarbyteUpgrade.Contains(item.buffType) && CustomDataPotionCheck(item);
        }
    }
}