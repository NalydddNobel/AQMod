using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Aequus.Tiles
{
    public class RecyclingMachineTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TERecyclingMachine>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
            DustType = DustID.Stone;
            AddMapEntry(new Color(140, 103, 103), CreateMapEntryName("RecyclingMachine"));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<RecyclingMachine>());
            ModContent.GetInstance<TERecyclingMachine>().Kill(i, j);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            var recycling = GetTileEntity(i, j);
            if (recycling == null || recycling.timeLeft > 0)
            {
                return false;
            }
            return TERecyclingMachine.GetUsableItem(Main.LocalPlayer) != null;
        }

        public override void MouseOver(int i, int j)
        {
            var recycling = GetTileEntity(i, j);
            if (recycling == null || recycling.timeLeft > 0)
            {
                return;
            }

            var player = Main.LocalPlayer;
            Item hoverItem;
            if (recycling.item != null && !recycling.item.IsAir)
            {
                hoverItem = recycling.item;
            }
            else
            {
                hoverItem = TERecyclingMachine.GetUsableItem(Main.LocalPlayer);
            }
            if (hoverItem != null)
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = hoverItem.type;
            }
        }

        public override bool AutoSelect(int i, int j, Item item)
        {
            return RecyclingTable.Convert.ContainsKey(item.type);
        }

        public override bool RightClick(int i, int j)
        {
            return (GetTileEntity(i, j)?.Interact(Main.LocalPlayer)).GetValueOrDefault(false);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16);
            if (Main.tile[i, j].TileFrameY >= 36)
            {
                frame.Height = 18;
            }
            var drawCoordinates = new Vector2(i * 16f - Main.screenPosition.X, j * 16f - Main.screenPosition.Y) + AequusHelpers.TileDrawOffset;

            var recycling = GetTileEntity(i, j);
            if (recycling != null)
            {
                drawCoordinates += recycling.GetDrawOffset(i, j);
            }

            Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates.Floor(),
                frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (i == (recycling.Position.X + 1) && j == recycling.Position.Y)
            {
                if (recycling.timeLeft == 0 && recycling.item != null && !recycling.item.IsAir)
                {
                    InnerDrawChatBubble(recycling, recycling.item, drawCoordinates + new Vector2(0f, -4f + AequusHelpers.Wave(Main.GlobalTimeWrappedHourly, -4f, 0f)).Floor());
                }
            }

            return false;
        }

        public void InnerDrawChatBubble(TERecyclingMachine recylcing, Item item, Vector2 where)
        {
            var chatBubbleFrame = Images.StatusBubble.Value.Frame(horizontalFrames: Images.StatusBubbleFrames, frameX: 1);
            Main.spriteBatch.Draw(Images.StatusBubble.Value, where.Floor(),
                chatBubbleFrame, Color.White, 0f, new Vector2(chatBubbleFrame.Width / 2f, chatBubbleFrame.Height), 1f, SpriteEffects.None, 0f);

            Main.instance.LoadItem(item.type);

            item.GetItemDrawData(out var frame);
            var itemTexture = TextureAssets.Item[item.type].Value;

            var itemScale = 1f;
            int largestSide = frame.Width > frame.Height ? frame.Width : frame.Height;
            if (largestSide > 38)
            {
                itemScale = 38f / largestSide;
            }

            where.Y += chatBubbleFrame.Height / -2f - 2f;

            Main.spriteBatch.Draw(itemTexture, where.Floor(),
                null, Color.White, 0f, frame.Size() / 2f, itemScale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Attempts to get the recycling machine Tile Entity instance. Returns null if none is found.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public TERecyclingMachine GetTileEntity(int i, int j)
        {
            int x = i - Main.tile[i, j].TileFrameX % 36 / 18;
            int y = j - Main.tile[i, j].TileFrameY / 18;

            int index = ModContent.GetInstance<TERecyclingMachine>().Find(x, y);
            if (index == -1)
            {
                return null;
            }
            return (TERecyclingMachine)TileEntity.ByID[index];
        }
    }

    public class TERecyclingMachine : ModTileEntity
    {
        public ushort timeLeft;
        public Item item;

        private Vector2 drawOffset;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<RecyclingMachineTile>()
                && Main.tile[x, y].TileFrameX == 0 && Main.tile[x, y].TileFrameY == 0;
        }

        public override void OnKill()
        {
            if (item != null && !item.IsAir)
            {
                AequusItem.NewItemCloned(new EntitySource_TileBreak(Position.X, Position.Y), new Vector2(Position.X * 16 + 16f, Position.Y * 16 + 24f), item);
            }
        }

        public Vector2 GetDrawOffset(int x, int y)
        {
            if (timeLeft == 0)
            {
                return Vector2.Zero;
            }

            if (Main.tile[x, y].TileFrameX == 0 && Main.tile[x, y].TileFrameY == 0 && Aequus.GameWorldActive)
            {
                drawOffset = new Vector2(EffectsSystem.EffectRand.Rand(-2f, 2f), EffectsSystem.EffectRand.Rand(-1f, 1f));
                if (drawOffset.Length() < 1f)
                {
                    drawOffset.Normalize();
                }
            }
            return drawOffset;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i - 1, j - 2, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i - 1, j - 2);
        }

        public override void Update()
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                if (timeLeft == 0)
                {
                    RecyclingTable.Convert.TryGetValue(item.type, out var l);

                    item = ConvertItem(item, l);

                    Sync();

                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.MenuOpen, Position.X * 16, Position.Y * 16);
                    }
                }
                else
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if ((Main.GameUpdateCount % 10) == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item22.SoundId, Position.X * 16, Position.Y * 16, SoundID.Item22.Style, 0.6f, -1f);
                        }
                    }
                }
            }
        }

        public Item ConvertItem(Item item, List<int> conversionData)
        {
            item = item.Clone();

            if (conversionData == null)
            {
                item.SetDefaults(item.type + 1);
                return item;
            }

            item.SetDefaults(conversionData[Main.rand.Next(conversionData.Count)]);
            return item;
        }

        public bool Interact(Player player)
        {
            if (item != null || item?.IsAir == false)
            {
                if (timeLeft != 0)
                {
                    return false;
                }

                player.QuickSpawnClonedItem(new EntitySource_TileEntity(this), item, item.stack);
                item = null;
                Sync();

                return true;
            }
            var trash = GetUsableItem(player);
            if (trash != null)
            {
                UseItem(trash);
                return true;
            }
            return false;
        }

        public void UseItem(Item item)
        {
            if (this.item != null || this.item?.IsAir == false)
            {
                return;
            }

            this.item = item.Clone();

            item.stack--;
            if (item.stack <= 0)
            {
                item.TurnToAir();
            }

            timeLeft = 3600;

            SoundEngine.PlaySound(SoundID.Grab);

            Sync();
        }

        public static Item GetUsableItem(Player player)
        {
            return GetUsableItem(player.HeldItem, player.inventory, Main.InventoryItemSlotsCount);
        }
        public static Item GetUsableItem(Item held, Item[] inv, int invCount)
        {
            if (held != null && RecyclingTable.Convert.ContainsKey(held.type))
            {
                return held;
            }
            for (int i = 0; i < invCount; i++)
            {
                if (RecyclingTable.Convert.ContainsKey(inv[i].type))
                {
                    return inv[i];
                }
            }
            return null;
        }

        public override void SaveData(TagCompound tag)
        {
            if (item != null && !item.IsAir)
            {
                tag["Item"] = item;
                tag["TimeLeft"] = timeLeft;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Item"))
            {
                item = tag.Get<Item>("Item");
                timeLeft = tag.Get<ushort>("TimeLeft");
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (item != null)
            {
                writer.Write(true);

                ItemIO.Send(item, writer, writeStack: true);
                writer.Write(timeLeft);
                return;
            }

            writer.Write(false);
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                item = ItemIO.Receive(reader, readStack: true);
                timeLeft = reader.ReadUInt16();
                return;
            }

            item = null;
            timeLeft = 0;
        }

        public void Sync()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
        }
    }

    public class RecyclingTable : ILoadable
    {
        public static Dictionary<int, List<int>> Convert { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Convert = new Dictionary<int, List<int>>()
            {
                [ItemID.TinCan] = new List<int>()
                {
                    ItemID.CopperBar,
                    ItemID.TinBar,
                },
                [ItemID.OldShoe] = new List<int>()
                {
                    ItemID.Silk,
                    ItemID.Cobweb,
                    ItemID.HermesBoots,
                },
            };
        }

        void ILoadable.Unload()
        {
            Convert?.Clear();
            Convert = null;
        }
    }
}