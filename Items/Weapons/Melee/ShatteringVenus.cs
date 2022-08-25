using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Melee
{
    public class ShatteringVenus : ModItem
    {
        public const int maxSavedTiles = 6;
        public const int tileSlices = 6;

        public static RenderInfo[] renderInfo;

        public class ID
        {
            public const int TileLeft = 0;
            public const int TileRight = 1;
            public const int TileTopLeft = 2;
            public const int TileTopRight = 3;
            public const int TileMiddle = 4;
            public const int TileTopMiddle = 5;
        }

        public struct ItemInfo
        {
            public ushort[] tiles;

            public ItemInfo(ushort[] tileArr)
            {
                if (tileArr.Length != maxSavedTiles)
                    throw new Exception("Initalized incorrectly sized shattering venus array");
                tiles = tileArr;
            }

            public ItemInfo() : this(new ushort[maxSavedTiles])
            {
            }

            public ItemInfo Clone()
            {
                var arr = new ushort[maxSavedTiles];
                Array.Copy(tiles, arr, arr.Length);
                return new ItemInfo() { tiles = arr, };
            }

            public bool SendClientChanges(BinaryWriter writer, ItemInfo comparison)
            {
                for (int i = 0; i < maxSavedTiles; i++)
                {
                    if (tiles[i] != comparison.tiles[i])
                    {
                        for (int k = 0; k < maxSavedTiles; k++)
                        {
                            writer.Write(tiles[i]);
                        }
                        return true;
                    }
                }
                return false;
            }

            public static ItemInfo RecieveChanges(BinaryReader reader)
            {
                var arr = new ushort[maxSavedTiles];
                for (int i = 0; i < maxSavedTiles; i++)
                {
                    arr[i] = reader.ReadUInt16();
                }
                return new ItemInfo(arr);
            }
        }

        public struct RenderInfo
        {
            public int tileFrameX;
            public int tileFrameY;
            public int[] tileFrameSlicesY;
            public Point offset;
        }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void Load()
        {
            renderInfo = new RenderInfo[maxSavedTiles]
            {
                new RenderInfo() { offset = new Point(21, 44),
                    tileFrameX = 0, tileFrameY = 54,
                    tileFrameSlicesY = new int[tileSlices] { 0, 0, 0, 0, -1, -2, }  },

                new RenderInfo() { offset = new Point(33, 32),
                    tileFrameX = 22, tileFrameY = 76,
                    tileFrameSlicesY = new int[tileSlices] { 2, 1, 0, 0, 2, 2, } },

                new RenderInfo() { offset = new Point(27, 50),
                    tileFrameX = 0, tileFrameY = 54,
                    tileFrameSlicesY = new int[tileSlices] { -2, -2, -2, -1, -1, -1, }  },

                new RenderInfo() { offset = new Point(39, 38),
                    tileFrameX = 22, tileFrameY = 76,
                    tileFrameSlicesY = new int[tileSlices] { 2, 1, 0, 0, 0, 0, }  },

                new RenderInfo() { offset = new Point(25, 36),
                    tileFrameX = 180, tileFrameY = 58,
                    tileFrameSlicesY = new int[tileSlices] { 0, 0, 0, 0, 0, 0, }  },

                new RenderInfo() { offset = new Point(39, 50),
                    tileFrameX = 58, tileFrameY = 54,
                    tileFrameSlicesY = new int[tileSlices] { 0, 0, 0, 0, 0, 0, }  }
            };
        }

        public override void Unload()
        {
            renderInfo = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 70;
            Item.knockBack = 7f;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.DustDevilValue;
            Item.shootSpeed = 8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileID.PaladinsHammerFriendly;
            Item.autoReuse = true;
        }

        //public override bool? UseItem(Player player)
        //{
        //    for (int i = 0; i < Main.maxChests; i++)
        //    {
        //        if (Main.chest[i] != null && Main.chest[i]?.item.ContainsAny((i) => i.type == Type) == true)
        //        {
        //            player.Center = new Vector2(Main.chest[i].x, Main.chest[i].y) * 16f;
        //        }
        //    }
        //    return base.UseItem(player);
        //}

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Load();

            spriteBatch.End();
            Begin.GeneralEntities.Begin(spriteBatch, Main.UIScaleMatrix);

            try
            {
                var info = Main.LocalPlayer.Aequus().shatteringVenus;

                    info.tiles = testRandomize();
                var corner = UI.ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
                corner += new Vector2(-16f, 16f) * Main.inventoryScale; // bottom corner of the item sprite

                corner += new Vector2(-2f, 2f) * Main.inventoryScale; // manual offset
                for (int i = 0; i < maxSavedTiles; i++)
                {
                    //if (i != Main.GameUpdateCount / 30 % maxSavedTiles)
                    //    continue;
                    int index = i;
                    switch (i)
                    {
                        case 0:
                            index = maxSavedTiles - 2;
                            break;
                        case 1:
                            index = maxSavedTiles - 1;
                            break;
                        case 2:
                            index = i - 2;
                            break;
                        case 3:
                        case 4:
                            index = i - 1;
                            break;
                        case 5:
                            index = ID.TileRight;
                            break;
                    }

                    if (info.tiles[index] != ushort.MaxValue)
                    {
                        var render = renderInfo[index];

                        Main.instance.LoadTiles(info.tiles[index]);
                        var texture = TextureAssets.Tile[info.tiles[index]].Value;

                        if (index == ID.TileMiddle)
                        {
                            spriteBatch.Draw(texture, corner + new Vector2(render.offset.X + 6f, -render.offset.Y - 6f) * scale, new Rectangle(render.tileFrameX, render.tileFrameY, 12, 12), drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                            spriteBatch.Draw(texture, corner + new Vector2(render.offset.X + 2f, -render.offset.Y + 2f) * scale, new Rectangle(render.tileFrameX, render.tileFrameY, 12, 12), drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        }
                        RenderTileSlices(spriteBatch, render, texture, corner, drawColor, scale);
                    }
                }
            }
            catch
            {

            }

            spriteBatch.End();
            Begin.UI.Begin(spriteBatch);
        }
        public void RenderTileSlices(SpriteBatch spriteBatch, RenderInfo render, Texture2D texture, Vector2 corner, Color drawColor, float scale)
        {
            for (int j = 0; j < tileSlices; j++)
            {
                float pixelY = 0f;
                var r = new Rectangle(render.tileFrameX + j * 2, render.tileFrameY, 2, 12);
                if (render.tileFrameSlicesY[j] < 0)
                {
                    r.Height += render.tileFrameSlicesY[j] * 2;
                }
                else if (render.tileFrameSlicesY[j] > 0)
                {
                    pixelY += render.tileFrameSlicesY[j] * 2;
                    r.Y += render.tileFrameSlicesY[j] * 2;
                    r.Height -= render.tileFrameSlicesY[j] * 2;
                }
                spriteBatch.Draw(texture, corner + new Vector2(render.offset.X + j * 2, -render.offset.Y + pixelY) * scale, r, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public ushort[] testRandomize()
        {
            var allowedTypes = new List<ushort>();

            AequusEffects.EffectRand.SetRand((int)Main.GameUpdateCount / 120);
            for (int i = 0; i < Main.maxTileSets; i++)
            {
                if (!Main.tileFrameImportant[i] && Main.tileSolid[i])
                {
                    allowedTypes.Add((ushort)i);
                }
            }

            var arr = new ushort[maxSavedTiles];
            for (int i = 0; i < maxSavedTiles; i++)
            {
                arr[i] = allowedTypes[(int)AequusEffects.EffectRand.Rand(allowedTypes.Count)];
            }
            return arr;
        }
    }
}