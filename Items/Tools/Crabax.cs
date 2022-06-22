using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools
{
    public class Crabax : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 20;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.axe = 25; // has the highest axe power
            Item.tileBoost = 5;
            Item.value = Item.sellPrice(gold: 2);
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.expert = true;
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<CrabaxPlayer>().crabax = Item;
        }
    }

    public class CrabaxPlayer : ModPlayer
    {
        public Item crabax;

        public override void Load()
        {
            On.Terraria.Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += Player_ItemCheck_UseMiningTools_ActuallyUseMiningTool;
        }
        private static void Player_ItemCheck_UseMiningTools_ActuallyUseMiningTool(On.Terraria.Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig, Player Player, Item sItem, out bool canHitWalls, int x, int y)
        {
            orig(Player, sItem, out canHitWalls, x, y);
            if (Main.myPlayer == Player.whoAmI && !Main.mouseRight)
            {
                var crabax = Player.GetModPlayer<CrabaxPlayer>();
                if (crabax.crabax != null)
                {
                    var rectangle = new Rectangle((int)(Player.position.X + Player.width / 2) / 16, (int)(Player.position.Y + Player.height / 2) / 16, 30, 30);
                    rectangle.X -= rectangle.Width / 2;
                    rectangle.Y -= rectangle.Height / 2;
                    int hitCount = 0;
                    const int HitCountMax = 8;
                    if (rectangle.X > 10 && rectangle.X < Main.maxTilesX - 10 && rectangle.Y > 10 && rectangle.Y < Main.maxTilesY - 10)
                    {
                        for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
                        {
                            for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                            {
                                if (Main.tile[i, j].HasTile && Main.tileAxe[Main.tile[i, j].TileType])
                                {
                                    int tileID = Player.hitTile.HitObject(i, j, 1);
                                    if (Main.tile[i, j].TileType == TileID.PalmTree)
                                    {
                                        int treeStumpX = i;
                                        int treeStumpY = j;

                                        for (; Main.tile[treeStumpX, treeStumpY].HasTile && Main.tile[treeStumpX, treeStumpY].TileType == TileID.PalmTree && Main.tile[treeStumpX, treeStumpY + 1].TileType == TileID.PalmTree; treeStumpY++)
                                        {
                                        }

                                        i = treeStumpX + 2; // skips the current index and the next one, since this entire tree has been completed
                                        j = rectangle.Y;

                                        if (Player.tileTargetX == treeStumpX && Player.tileTargetY == treeStumpY)
                                        {
                                            break;
                                        }

                                        orig(Player, sItem, out _, treeStumpX, treeStumpY);
                                        continue;
                                    }
                                    else if (TileID.Sets.IsATreeTrunk[Main.tile[i, j].TileType])
                                    {
                                        int treeStumpX = i;
                                        int treeStumpY = j;

                                        if (Main.tile[treeStumpX, treeStumpY].TileFrameY >= 198 && Main.tile[treeStumpX, treeStumpY].TileFrameX == 44)
                                        {
                                            treeStumpX++;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].TileFrameX == 66 && Main.tile[treeStumpX, treeStumpY].TileFrameY <= 44)
                                        {
                                            treeStumpX++;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].TileFrameX == 44 && Main.tile[treeStumpX, treeStumpY].TileFrameY >= 132 && Main.tile[treeStumpX, treeStumpY].TileFrameY <= 176)
                                        {
                                            treeStumpX++;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].TileFrameY >= 198 && Main.tile[treeStumpX, treeStumpY].TileFrameX == 66)
                                        {
                                            treeStumpX--;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].TileFrameX == 88 && Main.tile[treeStumpX, treeStumpY].TileFrameY >= 66 && Main.tile[treeStumpX, treeStumpY].TileFrameY <= 110)
                                        {
                                            treeStumpX--;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].TileFrameX == 22 && Main.tile[treeStumpX, treeStumpY].TileFrameY >= 132 && Main.tile[treeStumpX, treeStumpY].TileFrameY <= 176)
                                        {
                                            treeStumpX--;
                                        }

                                        i = treeStumpX + 2; // skips the current index and the next one, since this entire tree has been completed
                                        j = rectangle.Y;

                                        for (; Main.tile[treeStumpX, treeStumpY].HasTile && TileID.Sets.IsATreeTrunk[Main.tile[treeStumpX, treeStumpY].TileType] &&
                                            TileID.Sets.IsATreeTrunk[Main.tile[treeStumpX, treeStumpY + 1].TileType]; treeStumpY++)
                                        {
                                        }

                                        if (Player.tileTargetX == treeStumpX && Player.tileTargetY == treeStumpY)
                                        {
                                            break;
                                        }

                                        orig(Player, sItem, out _, treeStumpX, treeStumpY);
                                        continue;
                                    }
                                    else
                                    {
                                        orig(Player, sItem, out _, i, j);
                                    }
                                }
                            }
                            if (hitCount > HitCountMax)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override void ResetEffects()
        {
            crabax = null;
        }
    }
}