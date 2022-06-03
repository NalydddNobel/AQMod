using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class Hooks : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            On.Terraria.Player.UpdateItemDye += IUpdateItemDye.Hook_UpdateItemDye;
        }


        void ILoadable.Unload()
        {
        }

        public interface IUpdateItemDye
        {
            void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem);

            internal static void Hook_UpdateItemDye(On.Terraria.Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
            {
                orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                if (armorItem is IUpdateItemDye armorDye)
                {
                    armorDye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                }
                if (armorItem is IUpdateItemDye dye)
                {
                    dye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                }
            }
        }
    }
}