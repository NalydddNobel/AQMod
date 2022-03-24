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

namespace AQMod.Items.Potions.Concoctions
{
    public class PotionofContainersTag : ModItem
    {
        public override string Texture => AQUtils.GetPath<PotionofContainers>();

        public override bool CloneNewInstances => true;

        public Item chest = null;

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
            chest = AQItem.GetDefault(ItemID.Chest);
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
                int chestIndex = GetChestIndex((chest?.createTile).GetValueOrDefault(TileID.Containers), (chest?.placeStyle).GetValueOrDefault(0));
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
            if (chest == null || ItemID.Sets.Deprecated[chest.type])
                return;
            Texture2D chestTexture = Main.itemTexture[chest.type];
            float iconScale = 0.4f * scale;
            Vector2 drawPos = position + frame.Size() / 2f * scale + new Vector2(2f + chestTexture.Width * iconScale, 2f + chestTexture.Height * iconScale);
            Main.spriteBatch.Draw(chestTexture, drawPos, null, new Color(250, 250, 250), 0f, chestTexture.Size() / 2f, iconScale, SpriteEffects.None, 0f);
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["Chest"] = chest,
            };
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                chest = tag.Get<Item>("Chest");
            }
            catch
            {
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(chest.type);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            int type = reader.ReadInt32();
            chest = new Item();
            chest.SetDefaults(type);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (chest.type == ItemID.None)
            {
                var text = AQMod.GetText("ContainerPotionError");
                tooltips.Add(new TooltipLine(mod, "ChestType", text) { overrideColor = Colors.RarityTrash });
            }
            else
            {
                var text = AQMod.GetText("ContainerPotionLink");
                text = string.Format(text, Lang.GetItemName(chest.type), chest.type);
                tooltips.Add(new TooltipLine(mod, "ChestType", text) { overrideColor = new Color(255, 255, 238, 255) });
            }
        }
    }
}