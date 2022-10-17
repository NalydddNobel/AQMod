using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public class GoblinTinkererRerolls : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            On.Terraria.Item.Prefix += Item_Prefix;
        }

        private static bool Item_Prefix(On.Terraria.Item.orig_Prefix orig, Item item, int pre)
        {
            if (pre == -2 && AequusWorld.tinkererRerolls > 0 && AequusHelpers.iterations == 0)
            {
                bool val = false;
                int finalPrefix = 0;
                int value = 0;
                var cloneItem = new Item();
                for (int i = 0; i < AequusWorld.tinkererRerolls; i++)
                {
                    AequusHelpers.iterations = i + 1;
                    cloneItem.SetDefaults(item.type);
                    val |= cloneItem.Prefix(pre);
                    int prefixValue = cloneItem.value / 5;
                    if (cloneItem.prefix == PrefixID.Ruthless)
                    {
                        prefixValue = (int)(prefixValue * 2.15f);
                    }
                    if ((cloneItem.pick > 0 || cloneItem.axe > 0 || cloneItem.hammer > 0) && cloneItem.prefix == PrefixID.Light)
                    {
                        prefixValue = (int)(prefixValue * 2.5f);
                    }
                    if (prefixValue > value || finalPrefix == 0)
                    {
                        finalPrefix = cloneItem.prefix;
                        value = prefixValue;
                    }
                }
                AequusHelpers.iterations = 0;
                if (val && finalPrefix > 0)
                {
                    return item.Prefix(finalPrefix);
                }
            }
            return orig(item, pre);
        }

        void ILoadable.Unload()
        {
            throw new System.NotImplementedException();
        }
    }
}
