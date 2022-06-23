using Aequus.Common.Networking;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            if (recycling.HasItem)
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
                recycling.UpdateSounds();
            }

            Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates.Floor(),
                frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (i == (recycling.Position.X + 1) && j == recycling.Position.Y)
            {
                if (recycling.timeLeft == 0 && recycling.HasItem)
                {
                    InnerDrawChatBubble(recycling, recycling.item, drawCoordinates + new Vector2(0f, -4f + AequusHelpers.Wave(Main.GlobalTimeWrappedHourly, -4f, 0f)).Floor());
                }
            }

            return false;
        }

        public void InnerDrawChatBubble(TERecyclingMachine recylcing, Item item, Vector2 where)
        {
            var chatBubbleFrame = Images.StatusBubble.Value.Frame(horizontalFrames: Images.StatusBubbleFramesX, frameX: 1);
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

            Main.spriteBatch.Draw(itemTexture, where.Floor() + new Vector2(2f, 2f) * itemScale,
                null, Color.Black * 0.4f, 0f, frame.Size() / 2f, itemScale, SpriteEffects.None, 0f);
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

        public float eventPlaySound;
        public bool eventPlaySoundBlip;

        private Vector2 drawOffset;

        public bool HasItem => this.item != null || this.item?.IsAir == false;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<RecyclingMachineTile>()
                && Main.tile[x, y].TileFrameX == 0 && Main.tile[x, y].TileFrameY == 0;
        }

        public override void OnKill()
        {
            if (HasItem)
            {
                AequusItem.NewItemCloned(new EntitySource_TileEntity(this), new Vector2(Position.X * 16 + 16f, Position.Y * 16 + 24f), item);
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
                drawOffset = new Vector2(AequusEffects.EffectRand.Rand(-2f, 2f), AequusEffects.EffectRand.Rand(-1f, 1f));
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

        public void UpdateSounds()
        {
            if (!Aequus.GameWorldActive)
            {
                return;
            }
            if (timeLeft > 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.GlobalTimeWrappedHourly - eventPlaySound > 0.16f)
                    {
                        SoundEngine.PlaySound(SoundID.Item22.WithVolume(0.6f).WithPitch(-1f), new Vector2(Position.X * 16f, Position.Y * 16f));
                        eventPlaySound = Main.GlobalTimeWrappedHourly;
                    }
                }
                if (timeLeft > 1)
                {
                    eventPlaySoundBlip = false;
                }
                if (!eventPlaySoundBlip && timeLeft <= 1 && HasItem)
                {
                    SoundEngine.PlaySound(SoundID.MenuOpen, new Vector2(Position.X * 16f, Position.Y * 16f));
                    eventPlaySoundBlip = true;
                }
            }
            //if (item != null)
            //{
            //    Main.NewText(item.ToString());
            //}
            //Main.NewText(timeLeft);
        }

        public override void Update()
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                if (timeLeft == 0)
                {
                    RecyclingTable.Convert.TryGetValue(item.type, out var l);

                    item = ConvertItem(item, l.FindAll((l2) => l2.CanObtain()));
                    AequusItem.NewItemCloned(new EntitySource_TileEntity(this), new Vector2(Position.X * 16f + 16f, Position.Y * 16f + 24f), item);
                    item = null;
                    Sync();
                }
            }
        }

        public Item ConvertItem(Item item, List<RecyclingTable.Info> conversionData)
        {
            item = item.Clone();

            if (conversionData == null)
            {
                item.SetDefaults(item.type + 1);
                return item;
            }

            var c = conversionData[Main.rand.Next(conversionData.Count)];
            item.SetDefaults(c.item);
            item.stack = c.RollStack();
            return item;
        }

        public bool Interact(Player player)
        {
            if (HasItem)
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
                Sync();
                return true;
            }
            return false;
        }

        public void UseItem(Item item)
        {
            if (HasItem)
            {
                return;
            }

            this.item = item.Clone();
            this.item.stack = 1;

            item.stack--;
            if (item.stack <= 0)
            {
                item.TurnToAir();
            }

            timeLeft = 3600;

            SoundEngine.PlaySound(SoundID.Grab);
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
            if (HasItem)
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
            PacketHandler.Send((p) =>
            {
                p.Write(item != null);
                p.Write(ID);
                if (item != null)
                {
                    ItemIO.Send(item, p, writeStack: true);
                    p.Write(timeLeft);
                }
            }, PacketType.SyncRecyclingMachine_CauseForSomeReasonNetRecieveIsntWorkingOnTileEntities);
        }

        public override void NetReceive(BinaryReader reader)
        {
        }

        public static void NetReceive2(BinaryReader reader)
        {
            bool hasItem = reader.ReadBoolean();
            int id = reader.ReadInt32();
            var recyclingMachine = (TERecyclingMachine)ByID[id];
            ByPosition[recyclingMachine.Position] = recyclingMachine;
            if (hasItem)
            {
                recyclingMachine.item = ItemIO.Receive(reader, readStack: true);
                recyclingMachine.timeLeft = reader.ReadUInt16();
                //if (Main.netMode == NetmodeID.MultiplayerClient)
                //{
                //    Main.NewText(recyclingMachine.item.ToString()); 
                //    AequusHelpers.dustDebug(recyclingMachine.Position.X, recyclingMachine.Position.Y);
                //}
                return;
            }

            recyclingMachine.item = null;
            recyclingMachine.timeLeft = 0;
        }

        public void Sync()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                Aequus.Instance.Logger.Debug("Syncing recycling machine tile");
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
            }
        }
    }

    public class RecyclingTable : ILoadable
    {
        public struct Info
        {
            public int item;
            public int minStack;
            public int maxStack;
            public Func<bool> canObtain;

            public Info(int item, int minStack, int maxStack, Func<bool> canObtain = null)
            {
                this.item = item;
                this.minStack = minStack;
                this.maxStack = maxStack;
                this.canObtain = canObtain;
            }
            public Info(int item, int stack, Func<bool> canObtain = null) : this(item, stack, stack, canObtain)
            {
            }
            public Info(int item, Func<bool> canObtain = null) : this(item, 1, 1, canObtain)
            {
            }

            public bool CanObtain()
            {
                return (canObtain?.Invoke()).GetValueOrDefault(true);
            }

            public int RollStack()
            {
                return Main.rand.Next(minStack, maxStack + 1);
            }

            public static implicit operator Info(int item)
            {
                return new Info(item);
            }
        }

        public static Dictionary<int, List<Info>> Convert { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Convert = new Dictionary<int, List<Info>>()
            {
                [ItemID.TinCan] = new List<Info>()
                {
                    new Info(ItemID.CopperBar, 4, 8),
                    new Info(ItemID.TinBar, 4, 8),
                },
                [ItemID.OldShoe] = new List<Info>()
                {
                    new Info(ItemID.Silk, 1, 3),
                    new Info(ItemID.Cobweb, 7, 24),
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