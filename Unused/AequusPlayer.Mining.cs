using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus {
    public partial class AequusPlayer : ModPlayer
    {
        public Item crabax;
        public int maxCrabaxChops;
        public Item silkPick;
        public Item silkHammer;
        public static Item SilkTouch;

        private delegate void ItemCheck_UseMiningTools_ActuallyUseMiningTool(Player player, Item sItem, out bool canHitWalls, int x, int y);

        private static ItemCheck_UseMiningTools_ActuallyUseMiningTool mineTileMethod;

        public void Load_MiningEffects()
        {
            Terraria.On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += Player_ItemCheck_UseMiningTools_ActuallyUseMiningTool;
            mineTileMethod = (ItemCheck_UseMiningTools_ActuallyUseMiningTool)Delegate.CreateDelegate(typeof(ItemCheck_UseMiningTools_ActuallyUseMiningTool),
                typeof(Player).GetMethod("ItemCheck_UseMiningTools_ActuallyUseMiningTool", Helper.LetMeIn));
        }
        private static void Player_ItemCheck_UseMiningTools_ActuallyUseMiningTool(Terraria.On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig, Player Player, Item sItem, out bool canHitWalls, int x, int y)
        {
            bool? customCanHitWalls = null;
            if (Main.myPlayer == Player.whoAmI && Helper.iterations == 0)
            {
                Helper.iterations++;
                if (!Player.GetModPlayer<AequusPlayer>().UseSpecialTools(sItem, x, y, ref customCanHitWalls))
                {
                    Helper.iterations = 0;
                    canHitWalls = customCanHitWalls ?? false;
                    return;
                }
                Helper.iterations = 0;
            }
            orig(Player, sItem, out canHitWalls, x, y);
            canHitWalls = customCanHitWalls ?? canHitWalls;
        }

        public bool UseSpecialTools(Item sItem, int x, int y, ref bool? canHitWalls)
        {
            Helper.iterations++;
            bool mineBlock = true;
            if (crabax != null && !Main.mouseRight && UseCrabax(sItem))
            {
                Helper.iterations = 0;
                if (sItem.pick == 0 && sItem.hammer == 0)
                    return true;
            }
            if (silkPick != null && UseSilkPick(sItem, x, y))
            {
                if (sItem.axe == 0 && sItem.hammer == 0)
                    return false;
                mineBlock = false;
            }
            if (silkHammer != null)
            {
                canHitWalls = false;
                if (UseSilkHammer(sItem, x, y))
                {
                    if (sItem.axe == 0 && sItem.hammer == 0)
                        return false;
                    mineBlock = false;
                }
            }
            return mineBlock;
        }

        public bool UseSilkHammer(Item sItem, int x, int y)
        {
            if (Main.tile[x, y].WallType > WallID.None)
            {
                //AequusTile.LoadEchoWalls();
                if (AequusTile.WallIDToItemID.TryGetValue(Main.tile[x, y].WallType, out int itemID))
                {
                    bool gen = WorldGen.gen;
                    WorldGen.gen = true;
                    try
                    {
                        WorldGen.KillWall(x, y);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y);
                        }
                        Player.ApplyItemTime(sItem, 0.5f);
                    }
                    catch
                    {
                    }
                    WorldGen.gen = gen;
                }
            }
            return false;
        }

        public bool UseSilkPick(Item sItem, int x, int y)
        {
            if (Main.tile[x, y].HasTile && !Main.tileAxe[Main.tile[x, y].TileType] && !Main.tileHammer[Main.tile[x, y].TileType])
            {
                if (!WorldGen.CanKillTile(x, y) || Main.tile[x, y].TileType == TileID.MysticSnakeRope || !PickaxePowerCriteria(Player, sItem, x, y))
                    return false;

                var tileObjectData = TileObjectData.GetTileData(Main.tile[x, y]);
                int itemID = GetSilkTouchTileItem(Main.tile[x, y], tileObjectData);
                if (itemID != 0)
                {
                    AchievementsHelper.CurrentlyMining = true;

                    int i = Item.NewItem(new EntitySource_TileBreak(x, y, "Aequus: Silk Touch"), new Rectangle(x * 16, y * 16, 16, 16), itemID);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
                    }

                    int netModeHackForSpawningProjectiles = Main.netMode;
                    Main.netMode = NetmodeID.MultiplayerClient;
                    try
                    {
                        WorldGen.KillTile(x, y);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y);
                        }
                        Player.ApplyItemTime(sItem);
                        Player.toolTime = (int)(Player.itemTime * Player.pickSpeed);
                    }
                    catch
                    {
                    }
                    Main.netMode = netModeHackForSpawningProjectiles;
                    WorldGen.SquareTileFrame(x, y, resetFrame: true);

                    AchievementsHelper.CurrentlyMining = false;
                    return true;
                }
            }
            return false;
        }
        public int GetSilkTouchTileItem(Tile tile, TileObjectData tileObjectData)
        {
            if (tile.TileType == TileID.Hive)
                return ItemID.Hive;

            int style = TileObjectData.GetTileStyle(tile);
            if (tile.TileType == TileID.Plants || tile.TileType == TileID.CorruptPlants || tile.TileType == TileID.CrimsonPlants || tile.TileType == TileID.HallowedPlants)
                style = tile.TileFrameX / 18;
            //Main.NewText(style);
            if (AequusTile.TileIDToItemID.TryGetValue(new TileKey(tile.TileType, style == -1 ? 0 : style), out int value) && ContentSamples.ItemsByType[value].consumable)
            {
                return value;
            }
            if (AequusTile.TileIDToItemID.TryGetValue(new TileKey(tile.TileType, 0), out int defaultValue) && ContentSamples.ItemsByType[defaultValue].consumable)
            {
                return defaultValue;
            }
            return 0;
        }

        public bool UseCrabax(Item sItem)
        {
            var rectangle = new Rectangle((int)(Player.position.X + Player.width / 2) / 16, (int)(Player.position.Y + Player.height / 2) / 16, 30, 30);
            rectangle.X -= rectangle.Width / 2;
            rectangle.Y -= rectangle.Height / 2;
            int hitCount = 0;
            if (rectangle.X > 10 && rectangle.X < Main.maxTilesX - 10 && rectangle.Y > 10 && rectangle.Y < Main.maxTilesY - 10)
            {
                for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
                {
                    for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                    {
                        if (Main.tile[i, j].HasTile && Main.tileAxe[Main.tile[i, j].TileType])
                        {
                            int damage = Player.hitTile.HitObject(i, j, 1);
                            if (MineAxeTile(i, j, sItem, out int stumpX, out int stumpY))
                            {
                                // skips to the next lane
                                i = stumpX + 2;
                                j = rectangle.Y;
                            }
                        }
                    }
                    if (hitCount > maxCrabaxChops)
                    {
                        break;
                    }
                }
            }
            return hitCount > 0;
        }
        public bool MineAxeTile(int i, int j, Item sItem, out int stumpX, out int stumpY)
        {
            stumpX = i;
            stumpY = j;
            if (Main.tile[i, j].TileType == TileID.PalmTree)
            {
                for (; Main.tile[stumpX, stumpY].HasTile && Main.tile[stumpX, stumpY].TileType == TileID.PalmTree && Main.tile[stumpX, stumpY + 1].TileType == TileID.PalmTree; stumpY++)
                {
                }

                if (Player.tileTargetX == stumpX && Player.tileTargetY == stumpY)
                {
                    return true;
                }

                MineTile(Player, stumpX, stumpY, sItem);
                //orig(Player, sItem, out _, treeStumpX, treeStumpY);
                return true;
            }
            else if (TileID.Sets.IsATreeTrunk[Main.tile[i, j].TileType])
            {
                if (Main.tile[stumpX, stumpY].TileFrameY >= 198 && Main.tile[stumpX, stumpY].TileFrameX == 44)
                {
                    stumpX++;
                }
                if (Main.tile[stumpX, stumpY].TileFrameX == 66 && Main.tile[stumpX, stumpY].TileFrameY <= 44)
                {
                    stumpX++;
                }
                if (Main.tile[stumpX, stumpY].TileFrameX == 44 && Main.tile[stumpX, stumpY].TileFrameY >= 132 && Main.tile[stumpX, stumpY].TileFrameY <= 176)
                {
                    stumpX++;
                }
                if (Main.tile[stumpX, stumpY].TileFrameY >= 198 && Main.tile[stumpX, stumpY].TileFrameX == 66)
                {
                    stumpX--;
                }
                if (Main.tile[stumpX, stumpY].TileFrameX == 88 && Main.tile[stumpX, stumpY].TileFrameY >= 66 && Main.tile[stumpX, stumpY].TileFrameY <= 110)
                {
                    stumpX--;
                }
                if (Main.tile[stumpX, stumpY].TileFrameX == 22 && Main.tile[stumpX, stumpY].TileFrameY >= 132 && Main.tile[stumpX, stumpY].TileFrameY <= 176)
                {
                    stumpX--;
                }

                for (; Main.tile[stumpX, stumpY].HasTile && TileID.Sets.IsATreeTrunk[Main.tile[stumpX, stumpY].TileType] &&
                    TileID.Sets.IsATreeTrunk[Main.tile[stumpX, stumpY + 1].TileType]; stumpY++)
                {
                }

                if (Player.tileTargetX == stumpX && Player.tileTargetY == stumpY)
                {
                    return true;
                }

                MineTile(Player, stumpX, stumpY, sItem);
                return true;
            }
            else
            {
                MineTile(Player, i, j, sItem);
            }
            return false;
        }

        public void ResetEffects_MiningEffects()
        {
            crabax = null;
            silkPick = null;
            silkHammer = null;
            maxCrabaxChops = 8;
        }

        public static bool PickaxePowerCriteria(Player player, Item item, int x, int y)
        {
            var inv = player.inventory;
            player.inventory = new Item[inv.Length];
            for (int i = 0; i < inv.Length; i++)
            {
                player.inventory[i] = item;
            }
            bool value = false;
            try
            {
                value = player.HasEnoughPickPowerToHurtTile(x, y);
            }
            catch
            {
            }
            player.inventory = inv;
            return value;
        }
        public static void MineTile(Player player, int x, int y, out bool canHitWalls, Item sItem = null)
        {
            mineTileMethod.Invoke(player, sItem ?? player.HeldItem, out canHitWalls, x, y);
        }
        public static void MineTile(Player player, int x, int y, Item sItem = null)
        {
            MineTile(player, x, y, out bool _, sItem);
        }
    }
}