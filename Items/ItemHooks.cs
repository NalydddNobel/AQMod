using Aequus.Common.ModPlayers;
using Aequus.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class ItemHooks : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            On.Terraria.Player.PlaceThing_Tiles_BlockPlacementForAssortedThings += ICustomCanPlace.Player_PlaceThing_Tiles_BlockPlacementForAssortedThings;
            On.Terraria.Player.UpdateItemDye += IUpdateItemDye.Player_UpdateItemDye;
        }


        void ILoadable.Unload()
        {
        }

        public interface IOnSpawnProjectile
        {
            void OnSpawnProjectile(Projectile projectile, AequusProjectile aequusProjectile, IEntitySource source);
        }

        public interface ICustomCanPlace
        {
            bool? CheckCanPlace(Player player, int i, int j);

            public static bool BubbleTilePlacement(int i, int j)
            {
                return Main.tile[i + 1, j].HasTile || Main.tile[i + 1, j].WallType > 0
                    || Main.tile[i - 1, j].HasTile || Main.tile[i - 1, j].WallType > 0
                    || Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].WallType > 0
                    || Main.tile[i, j - 1].HasTile || Main.tile[i, j - 1].WallType > 0;
            }

            internal static bool Player_PlaceThing_Tiles_BlockPlacementForAssortedThings(On.Terraria.Player.orig_PlaceThing_Tiles_BlockPlacementForAssortedThings orig, Player self, bool canPlace)
            {
                if (self.inventory[self.selectedItem].ModItem is ItemHooks.ICustomCanPlace customCanPlace)
                {
                    var flag = customCanPlace.CheckCanPlace(self, Player.tileTargetX, Player.tileTargetY);
                    if (flag.HasValue)
                    {
                        return flag.Value;
                    }
                }
                return orig(self, canPlace);
            }
        }

        public interface IModifyFishingPower
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="player"></param>
            /// <param name="fishing"></param>
            /// <param name="fishingRod"></param>
            /// <param name="fishingLevel"></param>
            void ModifyFishingPower(Player player, PlayerFishing fishing, Item fishingRod, ref float fishingLevel);
        }

        public interface IUpdateVoidBag
        {
            void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank);
        }

        public interface IUpdateItemDye
        {
            void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="orig"></param>
            /// <param name="self"></param>
            /// <param name="isNotInVanitySlot"></param>
            /// <param name="isSetToHidden"></param>
            /// <param name="armorItem">If you are an equipped item, this is you.</param>
            /// <param name="dyeItem">If you are a dye, this is you.</param>
            internal static void Player_UpdateItemDye(On.Terraria.Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
            {
                orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                if (armorItem.ModItem is IUpdateItemDye armorDye)
                {
                    armorDye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                }
                if (dyeItem.ModItem is IUpdateItemDye dye)
                {
                    dye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                }
            }
        }
    }
}