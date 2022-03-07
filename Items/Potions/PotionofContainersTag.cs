using AQMod.Items.Potions;
using AQMod.Items.Recipes;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace AQMod.Items.Tools
{
    public class PotionofContainersTag : ModItem
    {
        public override string Texture => typeof(PotionofContainers).Namespace.Replace('.', '/') + "/" + nameof(PotionofContainers);

        /// <summary>
        /// Links the Item ID of a chest item to a Tile ID of a chest tile
        /// </summary>
        public struct ContainerTag
        {
            public readonly ushort chestItemID;
            public ushort chestTileID { get; private set; }
            public byte chestTileStyle { get; private set; }

            /// <summary>
            /// A container tag that is linked to a wooden chest
            /// </summary>
            public static ContainerTag Default => new ContainerTag(ItemID.Chest, TileID.Containers, 0);

            /// <summary>
            /// A container tag that is linked to a golden chest, given to container tags that don't load properly
            /// </summary>
            public static ContainerTag Error => new ContainerTag(ItemID.GoldChest, TileID.Containers, byte.MaxValue);

            public static ContainerTag FromKey(string key)
            {
                try
                {
                    if (key[0] == '0')
                    {
                        string id = "";
                        for (int i = 2; i < key.Length - 1 && key[i] != ':' && key[i] != ';'; i++)
                        {
                            id += key[i];
                        }
                        int itemType = int.Parse(id);
                        if (itemType >= Main.maxItemTypes)
                        {
                            return Error;
                        }
                        else
                        {
                            return new ContainerTag(itemType);
                        }
                    }
                    else
                    {
                        string mod = "";
                        int cursor = 2;
                        for (; cursor < key.Length - 1 && key[cursor] != ':'; cursor++)
                        {
                            mod += key[cursor];
                        }
                        cursor++;
                        string name = "";
                        for (; cursor < key.Length - 1 && key[cursor] != ':' && key[cursor] != ';'; cursor++)
                        {
                            name += key[cursor];
                        }
                        var modInstance = ModLoader.GetMod(mod);
                        int itemType = modInstance.ItemType(name);
                        if (itemType < Main.maxItemTypes)
                        {
                            return Error;
                        }
                        else
                        {
                            return new ContainerTag(itemType);
                        }
                    }
                }
                catch
                {
                    return Error;
                }
            }

            internal ContainerTag(int item, int tileType, int tileStyle)
            {
                chestItemID = (ushort)item;
                chestTileID = (ushort)tileType;
                chestTileStyle = (byte)tileStyle;
            }

            public ContainerTag(int item)
            {
                chestItemID = (ushort)item;
                chestTileID = TileID.Containers;
                chestTileStyle = byte.MaxValue;
                SetupTag();
            }

            public string GetKey()
            {
                if (chestItemID < Main.maxItemTypes)
                {
                    return "0:" + chestItemID + ";";
                }
                else
                {
                    var item = new Item();
                    item.netDefaults(chestItemID);
                    return "1:" + item.modItem.mod.Name + ":" + item.modItem.Name + ";";
                }
            }

            public bool SetupTag()
            {
                var item = new Item();
                item.netDefaults(chestItemID);
                if (!Main.tileContainer[chestTileID] || Main.tileTable[chestTileID])
                    return false;
                chestTileID = (ushort)item.createTile;
                chestTileStyle = (byte)item.placeStyle;
                return true;
            }
        }

        public override bool CloneNewInstances => true;

        public ContainerTag chestTag = ContainerTag.Default;

        private bool TryGetChestParameters(out ushort tile, out int style)
        {
            if (chestTag.chestItemID <= 0)
            {
                tile = 0;
                style = -1;
                return false;
            }
            var item = new Item();
            item.SetDefaults(chestTag.chestItemID);
            tile = (ushort)item.createTile;
            style = item.placeStyle;
            return Main.tileContainer[tile] && !Main.tileSolidTop[tile];
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(silver: 80);
        }

        private static int GetChestIndex(int type, int style)
        {
            for (int i = 0; i < Main.maxChests * 10; i++)
            {
                int index = Main.rand.Next(Main.maxChests);
                if (Main.chest[index] != null)
                {
                    var c = Main.chest[index];
                    var t = Framing.GetTileSafely(c.x, c.y);
                    if (t.type != type || TileObjectData.GetTileStyle(t) != style ||
                        Main.tileTable[t.type] || Chest.isLocked(c.x, c.y) || t.lava() ||
                        Main.wallDungeon[t.wall] && !NPC.downedBoss3 ||
                        t.wall == WallID.LihzahrdBrickUnsafe && !NPC.downedPlantBoss)
                    {
                        continue;
                    }
                    return index;
                }
            }
            return -1;
        }

        public override void UseStyle(Player player)
        {
            if (player.itemTime == 0)
            {
                player.itemTime = (int)(item.useTime / PlayerHooks.TotalUseTimeMultiplier(player, item));
            }
            else if (player.itemTime == (int)(item.useTime / PlayerHooks.TotalUseTimeMultiplier(player, item)) / 2)
            {
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GoldCoin);
                    Main.dust[d].velocity = player.velocity * 0.2f;
                }
                byte style = chestTag.chestTileStyle;
                if (style == 255)
                    style = 1;
                int chestIndex = GetChestIndex(chestTag.chestTileID, style);
                if (chestIndex == -1)
                    return;
                player.PrepareForTeleport();
                Chest chest1 = Main.chest[chestIndex];
                item.stack--;
                if (item.stack <= 0)
                    item.TurnToAir();
                player.Teleport(new Vector2((chest1.x + 1) * 16f, (chest1.y - 1) * 16f), -1);
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GoldCoin);
                    Main.dust[d].velocity = player.velocity * 0.2f;
                }
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (chestTag.chestItemID <= 0 || ItemID.Sets.Deprecated[chestTag.chestItemID])
                return;
            Texture2D chestTexture = Main.itemTexture[chestTag.chestItemID];
            float iconScale = 0.4f * scale;
            Vector2 drawPos = position + frame.Size() / 2f * scale + new Vector2(2f + chestTexture.Width * iconScale, 2f + chestTexture.Height * iconScale);
            Main.spriteBatch.Draw(chestTexture, drawPos, null, new Color(250, 250, 250), 0f, chestTexture.Size() / 2f, iconScale, SpriteEffects.None, 0f);
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["chestTag"] = chestTag.GetKey(),
            };
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                if (tag.ContainsKey("chestBait"))
                {
                    int legacyTag = tag.GetInt("chestBait");
                    chestTag = ContainerTag.FromKey("0:" + legacyTag);
                }
                else
                {
                    string key = tag.GetString("chestTag");
                    chestTag = ContainerTag.FromKey(key);
                }
            }
            catch
            {
                chestTag = ContainerTag.Error;
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(chestTag.chestItemID);
            writer.Write(chestTag.chestTileID);
            writer.Write(chestTag.chestTileStyle);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            ushort type = reader.ReadUInt16();
            ushort tileID = reader.ReadUInt16();
            byte style = reader.ReadByte();
            chestTag = new ContainerTag(type, tileID, style);
        }

        private static Color GetTextColor(float time)
        {
            int value = (int)(Math.Sin(time) * 10.0);
            return new Color(255 - value, 225 + value * 3, 180, 255);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (chestTag.chestItemID <= 0)
                return;
            if (chestTag.chestTileStyle == byte.MaxValue)
            {
                var text = AQText.ModText("Common.ContainerPotionImproperLoad").Value;
                tooltips.Add(new TooltipLine(mod, "ChestType", text) { overrideColor = Colors.RarityTrash });
            }
            else
            {
                var text = AQText.ModText("Common.ContainerPotionLink").Value;
                text = string.Format(text, Lang.GetItemName(chestTag.chestItemID), chestTag.chestItemID);
                tooltips.Add(new TooltipLine(mod, "ChestType", text) { overrideColor = GetTextColor(Main.GlobalTime * 6f) });
            }
        }

        public override void AddRecipes()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = new Item();
                item.SetDefaults(i);
                if (item.createTile >= TileID.Containers && Main.tileContainer[item.createTile] && !Main.tileSolidTop[item.createTile])
                    ContainersPotionRecipe.ConstructRecipe(i, this);
            }
        }
    }
}