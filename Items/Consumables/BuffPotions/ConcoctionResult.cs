using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Consumables.BuffPotions
{
    public abstract class ConcoctionResult : ModItem
    {
        public Item original;

        protected override bool CloneNewInstances => true;

        public override void SaveData(TagCompound tag)
        {
            tag["OriginalItem"] = original;
        }

        public override void LoadData(TagCompound tag)
        {
            original = tag.Get<Item>("OriginalItem");
            SetPotion();
        }

        public virtual void SetPotion()
        {
            Item.maxStack = original.maxStack;
            Item.buffType = original.buffType;
            Item.buffTime = original.buffTime;
            Item.potion = original.potion;
        }

        public override bool CanStack(Item item2)
        {
            return (item2.ModItem as StuffedConcoction).original.type == original.type;
        }
    }
}