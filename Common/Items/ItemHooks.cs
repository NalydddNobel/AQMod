using Aequus.Projectiles;
using Terraria.DataStructures;

namespace Aequus.Common.Items;
public class ItemHooks : ILoadable {
    void ILoadable.Load(Mod mod) {
        On_Player.ApplyPotionDelay += IApplyPotionDelay.On_Player_ApplyPotionDelay;
        On_NPC.BigMimicSummonCheck += ICheckBigMimicSummon.NPC_BigMimicSummonCheck;
        On_Player.PlaceThing_Tiles_BlockPlacementForAssortedThings += ICustomCanPlace.Player_PlaceThing_Tiles_BlockPlacementForAssortedThings;
        On_Player.UpdateItemDye += IUpdateItemDye.Player_UpdateItemDye;
        On_PopupText.NewText_PopupTextContext_Item_int_bool_bool += IHookPickupText.PopupText_NewText_PopupTextContext_Item_int_bool_bool;
    }

    void ILoadable.Unload() {
    }

    public interface ICustomNameTagPrice {
        int GetNameTagPrice(AequusItem aequusItem);
    }

    public interface IApplyPotionDelay {
        bool ApplyPotionDelay(Player player);

        internal static void On_Player_ApplyPotionDelay(On_Player.orig_ApplyPotionDelay orig, Player player, Item sItem) {
            if (sItem.ModItem is IApplyPotionDelay applyPotionDelay && applyPotionDelay.ApplyPotionDelay(player)) {
                return;
            }
            orig(player, sItem);
        }
    }

    public interface IDrawSpecialItemDrop {
        void OnPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
    }

    public interface IRightClickOverrideWhenHeld {
        bool RightClickOverrideWhileHeld(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus);
    }

    public interface ICheckBigMimicSummon {
        bool Choose(int x, int y, int chest, int currentItemCount, Player user) {
            return currentItemCount <= 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x">X coordinate of the chest.</param>
        /// <param name="y">Y coordinate of the chest.</param>
        /// <param name="chest">Chest ID of the chest.</param>
        /// <param name="tileID">Tile ID of the chest.</param>
        /// <param name="tileStyle">Tile style of the chest.</param>
        /// <param name="itemCount">The amount of items inside of the chest.</param>
        /// <param name="user">The player.</param>
        /// <returns>Whether or not to destroy the chest.</returns>
        bool DestroyChest(int x, int y, int chest, ushort tileID, int tileStyle, int itemCount, Player user) {
            return TileID.Sets.BasicChest[Main.tile[x, y].TileType];
        }

        /// <summary>
        /// Runs on Singleplayer, Multiplayer Client, and the Server.
        /// </summary>
        /// <param name="x">X coordinate of the chest, which might be destroyed.</param>
        /// <param name="y">Y coordinate of the chest, which might be destroyed.</param>
        /// <param name="tileID">Tile ID of the chest, which might be destroyed.</param>
        /// <param name="tileStyle">Tile style of the chest, which might be destroyed.</param>
        /// <param name="itemCount">The amount of items inside of the chest, which might be destroyed.</param>
        /// <param name="user">The player.</param>
        void OnActivate(int x, int y, ushort tileID, int tileStyle, int itemCount, Player user);

        internal static bool NPC_BigMimicSummonCheck(On_NPC.orig_BigMimicSummonCheck orig, int x, int y, Player user) {
            int chest = Chest.FindChest(x, y);
            if (chest < 0) {
                return false;
            }
            ICheckBigMimicSummon chosenKey = null;
            int itemCount = 0;
            ushort tileID = Main.tile[Main.chest[chest].x, Main.chest[chest].y].TileType;
            int tileStyle = Main.tile[Main.chest[chest].x, Main.chest[chest].y].TileFrameX / 36;

            if (!TileID.Sets.BasicChest[tileID] || tileID == TileID.Containers && tileStyle >= 5 && tileStyle <= 6) {
                return false;
            }

            for (int i = 0; i < Chest.maxItems; i++) {
                if (Main.chest[chest].item[i] == null || Main.chest[chest].item[i].type <= ItemID.None) {
                    continue;
                }

                if (Main.chest[chest].item[i].ModItem is ICheckBigMimicSummon foundKey) {
                    chosenKey = foundKey;
                }
                itemCount += Main.chest[chest].item[i].stack;
                if (chosenKey?.Choose(x, y, chest, itemCount, user) == false) {
                    chosenKey = null;
                }
            }

            if (chosenKey == null) {
                return orig(x, y, user);
            }

            if (chosenKey.DestroyChest(x, y, chest, tileID, tileStyle, itemCount, user)) {
                if (Main.tile[x, y].TileFrameX % 36 != 0) {
                    x--;
                }
                if (Main.tile[x, y].TileFrameY % 36 != 0) {
                    y--;
                }
                int number = Chest.FindChest(x, y);
                for (int j = 0; j < 40; j++) {
                    Main.chest[chest].item[j] = new Item();
                }
                Chest.DestroyChest(x, y);
                for (int k = x; k <= x + 1; k++) {
                    for (int l = y; l <= y + 1; l++) {
                        if (TileID.Sets.BasicChest[Main.tile[k, l].TileType]) {
                            Main.tile[k, l].ClearTile();
                        }
                    }
                }
                int number2 = 1;
                if (Main.tile[x, y].TileType == TileID.Containers2) {
                    number2 = 5;
                }
                if (Main.tile[x, y].TileType >= TileID.Count) {
                    number2 = 101;
                }
                NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, number2, x, y, 0f, number, Main.tile[x, y].TileType);
                NetMessage.SendTileSquare(-1, x, y, 3);
            }
            chosenKey.OnActivate(x, y, tileID, tileStyle, itemCount, user);
            return false;
        }
    }

    public interface IHoverSlot {
        bool HoverSlot(Item[] inventory, int context, int slot);
    }

    public interface IHookPickupText {
        void OnPickupText(int index, PopupTextContext context, int stack, bool noStack, bool longText);

        internal static int PopupText_NewText_PopupTextContext_Item_int_bool_bool(On_PopupText.orig_NewText_PopupTextContext_Item_int_bool_bool orig, PopupTextContext context, Item newItem, int stack, bool noStack, bool longText) {
            int original = orig(context, newItem, stack, noStack, longText);
            if (newItem.ModItem is IHookPickupText hookPickupText) {
                hookPickupText.OnPickupText(original, context, stack, noStack, longText);
            }
            return original;
        }
    }

    public interface ISetbonusDoubleTap {
        void OnDoubleTap(Player player, AequusPlayer aequus, int keyDir);
    }

    public interface IPreDrawPlayer {
        void PreDrawPlayer(Player player, AequusPlayer aequus, ref PlayerDrawSet drawInfo);
    }

    public interface IOnSpawnProjectile {
        public void OnShootProjectile(Projectile projectile, AequusProjectile aequusProjectile, IEntitySource source) {
        }
        public void OnSpawnProjectile(Projectile projectile, AequusProjectile aequusProjectile, IEntitySource source) {
        }
        public void InitalizeProjectile(Projectile projectile, AequusProjectile aequusProjectile) {
        }
    }

    public interface ICustomCanPlace {
        bool? CheckCanPlace(Player player, int i, int j);

        public static bool BubbleTilePlacement(int i, int j) {
            return Main.tile[i + 1, j].HasTile || Main.tile[i + 1, j].WallType > 0
                || Main.tile[i - 1, j].HasTile || Main.tile[i - 1, j].WallType > 0
                || Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].WallType > 0
                || Main.tile[i, j - 1].HasTile || Main.tile[i, j - 1].WallType > 0;
        }

        internal static bool Player_PlaceThing_Tiles_BlockPlacementForAssortedThings(On_Player.orig_PlaceThing_Tiles_BlockPlacementForAssortedThings orig, Player self, bool canPlace) {
            if (self.inventory[self.selectedItem].ModItem is ICustomCanPlace customCanPlace) {
                var flag = customCanPlace.CheckCanPlace(self, Player.tileTargetX, Player.tileTargetY);
                if (flag.HasValue) {
                    return flag.Value;
                }
            }
            return orig(self, canPlace);
        }
    }

    public interface IModifyFishingPower {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="fishing"></param>
        /// <param name="fishingRod"></param>
        /// <param name="fishingLevel"></param>
        void ModifyFishingPower(Player player, AequusPlayer fishing, Item fishingRod, ref float fishingLevel);
    }

    public interface IUpdateItemDye {
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
        internal static void Player_UpdateItemDye(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
            if (armorItem.ModItem is IUpdateItemDye armorDye) {
                armorDye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
            }
            if (dyeItem.ModItem is IUpdateItemDye dye) {
                dye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
            }
        }
    }
}