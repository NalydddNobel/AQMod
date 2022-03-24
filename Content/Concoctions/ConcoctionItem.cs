using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Concoctions
{
    public abstract class ConcoctionItem : ModItem
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
    }
}