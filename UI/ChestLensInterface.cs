using Aequus.Items.Weapons.Necromancy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;

namespace Aequus.UI {
    public class ChestLensInterface : AequusUserInterface
    {
        public class ChestLensInfo
        {
            public Point point;
            public Vector2 drawPoint;
            public float timer;

            public ChestLensInfo(Point point)
            {
                timer = 0f;
                this.point = point;
                drawPoint = point.ToWorldCoordinates(0f, 0f);
                drawPoint.X += TileObjectData.GetTileData(Main.tile[point]).Width * 16f / 2f;
            }

            public bool EnsureChestState()
            {
                return Chest.FindChest(point.X, point.Y) != -1;
            }

            public bool Update(GameTime gameTime, Point mousePoint)
            {
                int chestID = Chest.FindChest(point.X, point.Y);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    if (chestID == -1)
                    {
                        return false;
                    }
                    var c = Main.chest[chestID];
                    if (!c.IsSynced() || Main.GameUpdateCount % 30 == 0)
                    {
                        var p = Aequus.GetPacket(PacketType.RequestChestItems);
                        p.Write(Main.myPlayer);
                        p.Write(chestID);
                        p.Send();
                        return true;
                    }
                }
                if (point == mousePoint && (!(Main.LocalPlayer.chestX == point.X && Main.LocalPlayer.chestY == point.Y) || Main.LocalPlayer.chest == -1))
                {
                    if (timer < 5f)
                    {
                        timer += 1f;
                    }
                }
                else
                {
                    timer -= 1f;
                }
                return timer > 0f;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (timer <= 0f)
                    return;
                int chestID = Chest.FindChest(point.X, point.Y);
                if (chestID == -1)
                    return;
                var c = Main.chest[chestID];
                float opacity = (float)Math.Pow(timer / 5f, 2f);
                float inventoryScale = Main.inventoryScale;
                var texture = TextureAssets.InventoryBack13.Value;
                Main.inventoryScale = (0.4f + opacity * 0.2f) * Main.UIScale;
                var separation = new Vector2(texture.Width * Main.inventoryScale + 2f, texture.Height * Main.inventoryScale + 2f);
                int ten = 10;
                var topLeft = drawPoint - new Vector2(separation.X * ten / 2f, separation.Y * (Chest.maxItems / ten));
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    int x = i % ten;
                    int y = i / ten;

                    var drawCoords = topLeft + new Vector2(separation.X * x, separation.Y * y) - Main.screenPosition;
                    var clr = Main.mouseColor.HueAdd(GetHueForItem(c.item[i]));
                    Main.spriteBatch.Draw(texture, drawCoords, null, (clr * 0.2f).UseA(255) * opacity * 0.66f, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(TextureAssets.InventoryBack16.Value, drawCoords, null, clr * opacity * 0.66f, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
                    ItemSlotRenderer.Draw(spriteBatch, c.item[i], drawCoords, Color.White * opacity);
                }
                Main.inventoryScale = inventoryScale;
            }

            public float GetHueForItem(Item item)
            {
                if (item.shoot > ProjectileID.None)
                {
                    if (Main.projPet[item.shoot])
                    {
                        if (item.buffType > 0 && Main.lightPet[item.buffType])
                        {
                            return 0.3f;
                        }
                        return 0.25f;
                    }
                }
                if (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0)
                    return 0.9f;
                if (item.pick > 0)
                    return 0.8f;
                if (item.hammer > 0)
                    return 0.7f;
                if (item.axe > 0)
                    return 0.6f;
                if (item.damage > 0)
                {
                    if (item.ammo > 0)
                        return 0.7f;
                    if (item.DamageType == null)
                        return 0.1f;
                    if (item.DamageType.CountsAsClass(DamageClass.Melee))
                        return 0.2f;
                    if (item.DamageType.CountsAsClass(DamageClass.Ranged))
                        return 0.3f;
                    if (item.DamageType.CountsAsClass(DamageClass.Magic))
                        return 0.4f;
                    if (item.DamageType.CountsAsClass(DamageClass.Summon))
                        return 0.5f;
                    return 0.1f;
                }
                if (item.ModItem is SoulCandleBase)
                    return 0.5f;
                if (item.shoot > ProjectileID.None)
                    return 0.5f;
                if (item.buffType > 0 && item.buffTime > 0)
                    return 0.4f;
                if (item.makeNPC > 0)
                    return 0.25f;
                if (item.createTile >= TileID.Dirt || item.tileWand > -1)
                    return 0.2f;
                if (item.createWall >= WallID.None)
                    return 0.1f;
                if (item.accessory)
                {
                    if (item.defense > 0)
                        return 0.65f;
                    return 0.6f;
                }
                if (item.material)
                    return 0.3f;
                return 0f;
            }
        }

        public static bool Enabled;

        public static Dictionary<Point, ChestLensInfo> ChestLens { get; private set; }
        public override string Layer => AequusUI.InterfaceLayers.EntityHealthBars_16;
        public override InterfaceScaleType ScaleType => InterfaceScaleType.Game;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                ChestLens = new Dictionary<Point, ChestLensInfo>();
            }
        }
        public override void Unload()
        {
            ChestLens?.Clear();
            ChestLens = null;
        }

        public override void OnUIUpdate(GameTime gameTime)
        {
            if (!Enabled)
            {
                ChestLens.Clear();
                return;
            }

            var mousePoint = Point.Zero;
            if (WorldGen.InWorld(Player.tileTargetX, Player.tileTargetY, 30) && Main.tileContainer[Main.tile[Player.tileTargetX, Player.tileTargetY].TileType])
            {
                if (Main.LocalPlayer.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.QuickStackToNearbyChests))
                {
                    var tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
                    mousePoint = new Point(Player.tileTargetX - (tile.TileFrameX % 36) / 18, Player.tileTargetY - (tile.TileFrameY % 36) / 18);
                    if (!ChestLens.ContainsKey(mousePoint))
                    {
                        ChestLens.Add(mousePoint, new ChestLensInfo(mousePoint));
                    }
                }
            }

            if (ChestLens.Count <= 0)
                return;
            var removeCache = new List<Point>();
            foreach (var lens in ChestLens.Values)
            {
                if (!lens.EnsureChestState() || !lens.Update(gameTime, mousePoint))
                {
                    removeCache.Add(lens.point);
                    break;
                }
            }
            foreach (var p in removeCache)
            {
                ChestLens.Remove(p);
            }
        }

        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (!Enabled || ChestLens.Count <= 0)
                return true;
            foreach (var lens in ChestLens.Values)
            {
                if (!lens.EnsureChestState())
                {
                    break;
                }
                lens.Draw(spriteBatch);
            }
            return true;
        }
    }
}